// Copyright (c) 2008 Daniel Grunwald
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Tauron.Application.Wpf.Binding.WeakEvent
{
    /// <summary>
    ///     A class for managing a weak event.
    /// </summary>
    [DebuggerNonUserCode]
    public sealed class WeakEvent<T> where T : Delegate
    {
        private readonly List<EventEntry> _eventEntries = new List<EventEntry>();

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        static WeakEvent()
        {
            var invoke = typeof(T).GetMethod("Invoke");
            if (invoke == null || invoke.GetParameters().Length != 2)
                throw new ArgumentException("T must be a delegate type taking 2 parameters");
            var senderParameter = invoke.GetParameters()[0];
            if (senderParameter.ParameterType != typeof(object))
                throw new ArgumentException("The first delegate parameter must be of type 'object'");
            var argsParameter = invoke.GetParameters()[1];
            if (!argsParameter.ParameterType.IsSubclassOf(typeof(EventArgs)))
                throw new ArgumentException("The second delegate parameter must be derived from type 'EventArgs'. Type is " + argsParameter.ParameterType);
            if (invoke.ReturnType != typeof(void))
                throw new ArgumentException("The delegate return type must be void.");
        }

        public void Add(T eh)
        {
            if (eh == null) return;
            
            var d = eh;
            if (_eventEntries.Count == _eventEntries.Capacity)
                RemoveDeadEntries();
            var target = d.Target != null ? new WeakReference(d.Target) : null;
            _eventEntries.Add(new EventEntry(FastSmartWeakEventForwarderProvider.GetForwarder(d.Method), d.Method, target));
        }

        private void RemoveDeadEntries() 
            => _eventEntries.RemoveAll(ee => ee.TargetReference != null && !ee.TargetReference.IsAlive);


        public void Clear() 
            => _eventEntries.Clear();

        public void Remove(T eh)
        {
            if (eh == null) return;
            
            var d = eh;
            for (var i = _eventEntries.Count - 1; i >= 0; i--)
            {
                var entry = _eventEntries[i];
                if (entry.TargetReference != null)
                {
                    var target = entry.TargetReference.Target;
                    if (target == null)
                        _eventEntries.RemoveAt(i);
                    else if (target == d.Target && entry.TargetMethod == d.Method)
                    {
                        _eventEntries.RemoveAt(i);
                        break;
                    }
                }
                else
                {
                    if (d.Target != null || entry.TargetMethod != d.Method) continue;
                    
                    _eventEntries.RemoveAt(i);
                    break;
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        public void Raise(object sender, EventArgs e)
        {
            var needsCleanup = _eventEntries.ToArray().Aggregate(false, (current, ee) => current | ee.Forwarder(ee.TargetReference, sender, e));
            if (needsCleanup)
                RemoveDeadEntries();
        }

        [DebuggerNonUserCode]
        private struct EventEntry
        {
            public readonly FastSmartWeakEventForwarderProvider.ForwarderDelegate Forwarder;
            public readonly MethodInfo TargetMethod;
            public readonly WeakReference TargetReference;

            public EventEntry(FastSmartWeakEventForwarderProvider.ForwarderDelegate forwarder, MethodInfo targetMethod, WeakReference targetReference)
            {
                Forwarder = forwarder;
                TargetMethod = targetMethod;
                TargetReference = targetReference;
            }
        }
    }

    [DebuggerNonUserCode]
    internal static class FastSmartWeakEventForwarderProvider
    {
        private static readonly MethodInfo GetTarget = typeof(WeakReference).GetMethod("get_Target") ?? throw new InvalidOperationException();
        private static readonly Type[] ForwarderParameters = {typeof(WeakReference), typeof(object), typeof(EventArgs)};

        private static readonly Dictionary<MethodInfo, ForwarderDelegate> Forwarders = new Dictionary<MethodInfo, ForwarderDelegate>();

        internal static ForwarderDelegate GetForwarder(MethodInfo method)
        {
            lock (Forwarders)
            {
                if (Forwarders.TryGetValue(method, out var d))
                    return d;
            }

            if (method.DeclaringType?.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length != 0)
                throw new ArgumentException("Cannot create weak event to anonymous method with closure.");

            Debug.Assert(GetTarget != null);

            var dm = new DynamicMethod(
                "FastSmartWeakEvent", typeof(bool), ForwarderParameters, method.DeclaringType);

            var il = dm.GetILGenerator();

            if (!method.IsStatic)
            {
                il.Emit(OpCodes.Ldarg_0);
                il.EmitCall(OpCodes.Callvirt, GetTarget, null);
                il.Emit(OpCodes.Dup);
                var label = il.DefineLabel();
                il.Emit(OpCodes.Brtrue, label);
                il.Emit(OpCodes.Pop);
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Ret);
                il.MarkLabel(label);
            }

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_2);
            il.EmitCall(OpCodes.Call, method, null);
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Ret);

            var fd = (ForwarderDelegate) dm.CreateDelegate(typeof(ForwarderDelegate));
            lock (Forwarders)
            {
                Forwarders[method] = fd;
            }

            return fd;
        }

        internal delegate bool ForwarderDelegate(WeakReference wr, object sender, EventArgs e);
    }
}