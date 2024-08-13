using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InfoDataBase
{
  public static DataBase<BlockType, BlockInfo> BlockDataBase;
  public static DataBase<string, ItemInfo> ItemsDataBase;
  public static void InitBases()
  {
    BlockDataBase = new DataBase<BlockType, BlockInfo>("Blocks", block => block.type);
    ItemsDataBase= new DataBase<string,ItemInfo>("Items", item => item.ItemID);
  }
}
