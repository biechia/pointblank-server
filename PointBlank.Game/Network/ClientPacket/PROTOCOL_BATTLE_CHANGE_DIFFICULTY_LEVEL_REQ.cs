﻿using PointBlank.Core;
using PointBlank.Core.Models.Enums;
using PointBlank.Core.Models.Room;
using PointBlank.Core.Network;
using PointBlank.Game.Data.Model;
using PointBlank.Game.Network.ServerPacket;
using System;

namespace PointBlank.Game.Network.ClientPacket
{
  public class PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_REQ : ReceivePacket
  {
    public PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_REQ(GameClient client, byte[] data)
    {
      this.makeme(client, data);
    }

    public override void read()
    {
    }

    public override void run()
    {
      try
      {
        Account player = this._client._player;
        PointBlank.Game.Data.Model.Room room = player == null ? (PointBlank.Game.Data.Model.Room) null : player._room;
        if (room == null || room._state != RoomState.Battle || room.IngameAiLevel >= (byte) 10)
          return;
        Slot slot = room.getSlot(player._slotId);
        if (slot == null || slot.state != SlotState.BATTLE)
          return;
        if (room.IngameAiLevel <= (byte) 9)
          ++room.IngameAiLevel;
        using (PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_ACK difficultyLevelAck = new PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_ACK(room))
          room.SendPacketToPlayers((SendPacket) difficultyLevelAck, SlotState.READY, 1);
      }
      catch (Exception ex)
      {
        Logger.info("PROTOCOL_BATTLE_CHANGE_DIFFICULTY_LEVEL_REQ: " + ex.ToString());
      }
    }
  }
}
