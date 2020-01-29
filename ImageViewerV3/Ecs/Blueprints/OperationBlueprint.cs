﻿using System.Windows.Input;
using EcsRx.Blueprints;
using EcsRx.Components;
using EcsRx.Entities;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;

namespace ImageViewerV3.Ecs.Blueprints
{
    public sealed class OperationBlueprint : IBlueprint
    {
        private readonly StartOperationEvent _start;

        public OperationBlueprint(StartOperationEvent start) => _start = start;

        public void Apply(IEntity entity) 
            => entity.AddComponents(new IComponent[]{new OperationComponent{ IsActive = true, Message = _start.Name} });
    }
}