﻿namespace UltimateTerrains
{
    public class PositionZNode : IGeneratorNode
    {
        public bool Is2D {
            get { return true; }
        }

        public double Execute(double x, double y, double z, CallableNode flow)
        {
            return z;
        }
    }
}