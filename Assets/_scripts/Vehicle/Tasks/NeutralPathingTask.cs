﻿using System.Collections.Generic;

namespace RideShareLevel
{
    public class NeutralPathingTask : VehicleTask
    {
        public Queue<Connection> Path { get; private set; }

        public NeutralPathingTask(Vehicle vehicle, bool drawPath, Queue<Connection> path) : base(vehicle, drawPath)
        {
            Path = path;
        }

        public override bool IsComplete()
        {
            return Vehicle.PathIsComplete;
        }

        public override bool ShouldStart()
        {
            return true;
        }

        public override void Complete()
        {
            Vehicle.Despawn();
        }
    }
}