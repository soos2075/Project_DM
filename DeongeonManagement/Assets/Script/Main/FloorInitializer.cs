using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorInitializer : MonoBehaviour
{
    //void Start()
    //{
        
    //}



    public void NewGame_Init()
    {
        Init_EggFloor();
        Init_1Floor();
        //Init_2Floor();
        Init_3Floor();
        Init_4Floor();
        Init_5Floor();
    }


    enum CreateOption
    {
        Create,
        Replace,
        Return,
    }

    IPlacementable CreateObj(int floorIndex, Vector2Int posXY, string facilityName, CreateOption createOption = CreateOption.Create)
    {
        var tile = Main.Instance.Floor[floorIndex].GetRandomTile();
        Main.Instance.Floor[floorIndex].TileMap.TryGetValue(posXY, out tile);
        PlacementInfo info = new PlacementInfo(Main.Instance.Floor[floorIndex], tile);
        IPlacementable obj = null;

        //? 만약 지으려던 곳에 이미 퍼실리티가 존재한다면
        if (info.Place_Tile.Original != null && info.Place_Tile.Original as Facility != null) 
        {
            switch (createOption)
            {
                case CreateOption.Create: //? 그냥 통과
                    break;

                case CreateOption.Replace: //? 일단 기존꺼 제거
                    GameManager.Facility.RemoveFacility(info.Place_Tile.Original as Facility);
                    break;

                case CreateOption.Return: //? 기존꺼로 바로 리턴
                    return info.Place_Tile.Original;
            }
        }
        obj = GameManager.Facility.CreateFacility(facilityName, info);

        return obj;
    }

    IPlacementable CreateObj_OnlyOne(int floorIndex, Vector2Int posXY, string facilityName)
    {
        var tile = Main.Instance.Floor[floorIndex].GetRandomTile();
        Main.Instance.Floor[floorIndex].TileMap.TryGetValue(posXY, out tile);
        PlacementInfo info = new PlacementInfo(Main.Instance.Floor[floorIndex], tile);
        var obj = GameManager.Facility.CreateFacility_OnlyOne(facilityName, info);

        return obj;
    }




    #region 커스텀 새시작 보너스
    public void Init_FirstPlay_Bonus()
    {
        for (int k = 0; k < 8; k++)
        {
            BasementTile tile = Main.Instance.Floor[1].GetRandomTile();
            var info = new PlacementInfo(Main.Instance.Floor[1], tile);
            var obj = GameManager.Facility.CreateFacility("Herb_Low", info);
        }
        for (int k = 0; k < 5; k++)
        {
            BasementTile tile = Main.Instance.Floor[2].GetRandomTile();
            var info = new PlacementInfo(Main.Instance.Floor[2], tile);
            var facil = GameManager.Facility.CreateFacility("Mineral_Rock", info);
        }

        for (int k = 0; k < 2; k++)
        {
            BasementTile tile = Main.Instance.Floor[3].GetRandomTile();
            var info = new PlacementInfo(Main.Instance.Floor[3], tile);
            var facil = GameManager.Facility.CreateFacility("Mineral_Stone", info);
        }
        for (int k = 0; k < 2; k++)
        {
            BasementTile tile = Main.Instance.Floor[3].GetRandomTile();
            var info = new PlacementInfo(Main.Instance.Floor[3], tile);
            var obj = GameManager.Facility.CreateFacility("Herb_High", info);
        }
    }
    #endregion




    #region 각 Floor의 초기화 설정

    void Init_EggFloor()
    {
        Init_Egg();
        Init_Obstacle();
        Init_EggEntrance();
        Init_Statue();
    }




    void CreateObstacle(Define.DungeonFloor floor, RemoveableObstacle.SizeOption size, int x, int y, string categoty, string label)
    {
        BasementTile tile = Main.Instance.Floor[(int)floor].GetRandomTile_Size(x, y);

        if (tile == null)
        {
            Debug.Log($"건설불가 : {categoty} - {label}");
            return;
        }

        var info = new PlacementInfo(Main.Instance.Floor[(int)floor], tile);
        var obj = GameManager.Facility.CreateFacility("RemoveableObstacle", info);
        var ro = obj as RemoveableObstacle;
        ro.Set_ObstacleOption(size, categoty, label);
    }


    void Init_1Floor()
    {
        //CreateObstacle(Define.DungeonFloor.Floor_1, RemoveableObstacle.SizeOption._1x1, 1, 1, "Floor1", "2");
        CreateObstacle(Define.DungeonFloor.Floor_1, RemoveableObstacle.SizeOption._3x3, 3, 3, "Floor1", "5");

        CreateObstacle(Define.DungeonFloor.Floor_1, RemoveableObstacle.SizeOption._2x2, 2, 2, "Floor1", "4");

        CreateObstacle(Define.DungeonFloor.Floor_1, RemoveableObstacle.SizeOption._1x2, 1, 2, "Floor1", "1");

        CreateObstacle(Define.DungeonFloor.Floor_1, RemoveableObstacle.SizeOption._1x2, 1, 2, "Floor1", "3");

        CreateObstacle(Define.DungeonFloor.Floor_1, RemoveableObstacle.SizeOption._1x1, 1, 1, "Floor1", "2");

    }
    void Init_2Floor()
    {
        CreateObstacle(Define.DungeonFloor.Floor_2, RemoveableObstacle.SizeOption._1x2, 1, 2, "Floor3", "1");
    }


    void Init_3Floor()
    {
        CreateObstacle(Define.DungeonFloor.Floor_3, RemoveableObstacle.SizeOption._3x3, 3, 3, "Floor3", "7");

        CreateObstacle(Define.DungeonFloor.Floor_3, RemoveableObstacle.SizeOption._2x2, 2, 2, "Floor3", "3");
        //CreateObstacle(Define.DungeonFloor.Floor_3, RemoveableObstacle.SizeOption._2x2, 2, 2, "Floor3", "3");

        CreateObstacle(Define.DungeonFloor.Floor_3, RemoveableObstacle.SizeOption._2x2, 2, 2, "Floor3", "6");
        //CreateObstacle(Define.DungeonFloor.Floor_3, RemoveableObstacle.SizeOption._2x2, 2, 2, "Floor3", "6");

        CreateObstacle(Define.DungeonFloor.Floor_3, RemoveableObstacle.SizeOption._2x1, 2, 1, "Floor3", "5");
        //CreateObstacle(Define.DungeonFloor.Floor_3, RemoveableObstacle.SizeOption._2x1, 2, 1, "Floor3", "5");

        CreateObstacle(Define.DungeonFloor.Floor_3, RemoveableObstacle.SizeOption._1x2, 1, 2, "Floor3", "4");
        //CreateObstacle(Define.DungeonFloor.Floor_3, RemoveableObstacle.SizeOption._1x2, 1, 2, "Floor3", "4");
    }

    void Init_4Floor()
    {
        CreateObstacle(Define.DungeonFloor.Floor_4, RemoveableObstacle.SizeOption._3x3, 3, 3, "Floor4", "5");
        CreateObstacle(Define.DungeonFloor.Floor_4, RemoveableObstacle.SizeOption._3x3, 3, 3, "Floor4", "5");

        //CreateObstacle(Define.DungeonFloor.Floor_4, RemoveableObstacle.SizeOption._2x2, 2, 2, "Floor4", "2");
        CreateObstacle(Define.DungeonFloor.Floor_4, RemoveableObstacle.SizeOption._2x2, 2, 2, "Floor4", "2");

        CreateObstacle(Define.DungeonFloor.Floor_4, RemoveableObstacle.SizeOption._2x1, 2, 1, "Floor4", "4");
        //CreateObstacle(Define.DungeonFloor.Floor_4, RemoveableObstacle.SizeOption._2x1, 2, 1, "Floor4", "4");

        CreateObstacle(Define.DungeonFloor.Floor_4, RemoveableObstacle.SizeOption._1x2, 1, 2, "Floor4", "1");
        //CreateObstacle(Define.DungeonFloor.Floor_4, RemoveableObstacle.SizeOption._1x2, 1, 2, "Floor4", "1");


        CreateObstacle(Define.DungeonFloor.Floor_4, RemoveableObstacle.SizeOption._1x1, 1, 1, "Floor4", "3");
        CreateObstacle(Define.DungeonFloor.Floor_4, RemoveableObstacle.SizeOption._1x1, 1, 1, "Floor4", "3");
        //CreateObstacle(Define.DungeonFloor.Floor_4, RemoveableObstacle.SizeOption._1x1, 1, 1, "Floor4", "3");
    }
    void Init_5Floor()
    {
        CreateObstacle(Define.DungeonFloor.Floor_5, RemoveableObstacle.SizeOption._3x3, 3, 3, "Floor5", "4");
        CreateObstacle(Define.DungeonFloor.Floor_5, RemoveableObstacle.SizeOption._3x3, 3, 3, "Floor5", "4");

        //CreateObstacle(Define.DungeonFloor.Floor_5, RemoveableObstacle.SizeOption._2x2, 2, 2, "Floor5", "2");
        CreateObstacle(Define.DungeonFloor.Floor_5, RemoveableObstacle.SizeOption._2x2, 2, 2, "Floor5", "2");


        CreateObstacle(Define.DungeonFloor.Floor_5, RemoveableObstacle.SizeOption._1x2, 1, 2, "Floor5", "1");
        CreateObstacle(Define.DungeonFloor.Floor_5, RemoveableObstacle.SizeOption._1x2, 1, 2, "Floor5", "1");

        CreateObstacle(Define.DungeonFloor.Floor_5, RemoveableObstacle.SizeOption._1x2, 1, 2, "Floor5", "3");
        CreateObstacle(Define.DungeonFloor.Floor_5, RemoveableObstacle.SizeOption._1x2, 1, 2, "Floor5", "3");
    }



    public void Init_Egg()        //? 알 생성 (지금은 2*2인데 3*3으로 바꿀지 고민좀)
    {
        var egg = CreateObj_OnlyOne((int)Define.DungeonFloor.Egg, new Vector2Int(0, 2), "Egg_Lv1") as SpecialEgg;
        Main.Instance.EggObj = egg.GetObject();
        egg.SetEggData(GameManager.Facility.GetData("Egg_Lv1"));

        var sub1 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(0, 3), "Clone_Facility", CreateOption.Return) as Clone_Facility;
        var sub2 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(1, 2), "Clone_Facility", CreateOption.Return) as Clone_Facility;
        var sub3 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(1, 3), "Clone_Facility", CreateOption.Return) as Clone_Facility;
        sub1.OriginalTarget = egg;
        sub2.OriginalTarget = egg;
        sub3.OriginalTarget = egg;
    }

    void Init_Obstacle() //? 플레이어와 알 사이에 2칸도 그냥 설치불가능 지역으로 만들기
    { //? 한칸은 알 사이즈 늘리면서 먹었음
        //CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(1, 2), "Obstacle", CreateOption.Return);
        CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(2, 2), "Obstacle", CreateOption.Return);
    }


    void Init_EggEntrance() //? 전이진이 생길 위치
    {
        CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(12, 2), "Obstacle_Wall", CreateOption.Return);
        CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(11, 2), "Obstacle", CreateOption.Return);

        CreateObj((int)Define.DungeonFloor.Floor_3, new Vector2Int(2, 2), "Obstacle", CreateOption.Return);
        CreateObj((int)Define.DungeonFloor.Floor_4, new Vector2Int(2, 2), "Obstacle", CreateOption.Return);
        CreateObj((int)Define.DungeonFloor.Floor_5, new Vector2Int(6, 19), "Obstacle", CreateOption.Return);
    }

    void Init_Statue()
    {
        {
            //? 스태츄 - 1번자리 (우상단)
            var main = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(10, 4), "StatueBase", CreateOption.Return) as Statue;
            var sub1 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(10, 5), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;
            var sub2 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(11, 4), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;
            var sub3 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(11, 5), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;

            main.Set_StatueType(Statue.StatueType.Statue_Gold);
            sub1.OriginalTarget = main;
            sub2.OriginalTarget = main;
            sub3.OriginalTarget = main;
        }

        {
            //? 스태츄 - 2번자리 (우하단)
            var main = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(10, 0), "StatueBase", CreateOption.Return) as Statue;
            var sub1 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(10, 1), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;
            var sub2 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(11, 0), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;
            var sub3 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(11, 1), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;

            main.Set_StatueType(Statue.StatueType.Statue_Mana);
            sub1.OriginalTarget = main;
            sub2.OriginalTarget = main;
            sub3.OriginalTarget = main;
        }


        //? 3번 - 개
        if (UserData.Instance.FileConfig.Statue_Dog)
        {
            var main = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(7, 4), "StatueBase", CreateOption.Return) as Statue;
            var sub1 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(7, 5), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;
            var sub2 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(8, 4), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;
            var sub3 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(8, 5), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;

            main.Set_StatueType(Statue.StatueType.Statue_Dog);
            sub1.OriginalTarget = main;
            sub2.OriginalTarget = main;
            sub3.OriginalTarget = main;
        }
        else
        {
            CreateObj(0, new Vector2Int(7, 4), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(0, new Vector2Int(7, 5), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(0, new Vector2Int(8, 4), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(0, new Vector2Int(8, 5), "Obstacle_Wall", CreateOption.Replace);
        }

        //? 4번 - 드래곤마왕
        if (UserData.Instance.FileConfig.Statue_Dragon)
        {
            var main = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(7, 0), "StatueBase", CreateOption.Return) as Statue;
            var sub1 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(7, 1), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;
            var sub2 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(8, 0), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;
            var sub3 = CreateObj((int)Define.DungeonFloor.Egg, new Vector2Int(8, 1), "Clone_Facility_Wall", CreateOption.Return) as Clone_Facility_Wall;

            main.Set_StatueType(Statue.StatueType.Statue_Dragon);
            sub1.OriginalTarget = main;
            sub2.OriginalTarget = main;
            sub3.OriginalTarget = main;
        }
        else
        {
            CreateObj(0, new Vector2Int(7, 0), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(0, new Vector2Int(7, 1), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(0, new Vector2Int(8, 0), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(0, new Vector2Int(8, 1), "Obstacle_Wall", CreateOption.Replace);
        }

        //? 5번 자리
        CreateObj(0, new Vector2Int(4, 0), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(0, new Vector2Int(4, 1), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(0, new Vector2Int(5, 0), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(0, new Vector2Int(5, 1), "Obstacle_Wall", CreateOption.Replace);

        //? 6번 자리
        CreateObj(0, new Vector2Int(4, 4), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(0, new Vector2Int(4, 5), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(0, new Vector2Int(5, 4), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(0, new Vector2Int(5, 5), "Obstacle_Wall", CreateOption.Replace);
    }






    #endregion
}
