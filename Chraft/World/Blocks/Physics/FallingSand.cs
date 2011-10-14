﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chraft.Interfaces;
using Chraft.Utils;

namespace Chraft.World.Blocks.Physics
{
    public class FallingSand : BlockBasePhysics
    {
        protected byte BlockId;

        public FallingSand(WorldManager world, AbsWorldCoords pos) : base(world, pos)
        {
            Type = Net.Packets.AddObjectVehiclePacket.ObjectType.FallingSand;
            BlockId = (byte) BlockData.Blocks.Sand;
            Velocity = new Vector3(0, -0.4D, 0);
        }

        public override void Simulate()
        {
            int x = (int)Math.Floor(Position.X);
            int y = (int)Math.Floor(Position.Y);
            int z = (int)Math.Floor(Position.Z);
            byte blockId = World.GetBlockId(x, y, z);
            if (blockId != (byte)BlockData.Blocks.Air)
            {
                Stop();
                return;
            }

            if (Position.Y <= 1)
            {
                Stop();
                return;
            }

            Position = new AbsWorldCoords(Position.ToVector() + Velocity);
        }

        protected override void OnStop()
        {
            UniversalCoords currentBlockCoords = UniversalCoords.FromAbsWorld(Position.X, Position.Y, Position.Z);
            byte blockId = World.GetBlockId(currentBlockCoords);
            if (BlockHelper.Instance(blockId).IsAir)
            {
                World.Server.DropItem(World, currentBlockCoords, new ItemStack(BlockId, 1));
            }
            else
            {
                UniversalCoords aboveBlockCoords = UniversalCoords.FromAbsWorld(currentBlockCoords.WorldX,
                                                                             currentBlockCoords.WorldY + 1,
                                                                             currentBlockCoords.WorldZ);
                StructBlock aboveBlock = new StructBlock(aboveBlockCoords, BlockId, 0, World);
                BlockHelper.Instance(BlockId).Spawn(aboveBlock);
            }
            base.OnStop();
        }
    }
}