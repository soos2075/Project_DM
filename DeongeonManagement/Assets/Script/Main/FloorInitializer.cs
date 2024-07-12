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
        Init_2Floor();
        Init_3Floor();
        Init_4Floor();
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

            BasementTile tile = Main.Instance.Floor[0].GetRandomTile();
            var info = new PlacementInfo(Main.Instance.Floor[0], tile);
            var obj = GameManager.Facility.CreateFacility("Herb_Low", info);
        }
        for (int k = 0; k < 2; k++)
        {
            BasementTile tile = Main.Instance.Floor[1].GetRandomTile();
            var info = new PlacementInfo(Main.Instance.Floor[1], tile);
            var obj = GameManager.Facility.CreateFacility("Herb_High", info);
        }
        for (int k = 0; k < 5; k++)
        {
            BasementTile tile = Main.Instance.Floor[2].GetRandomTile();
            var info = new PlacementInfo(Main.Instance.Floor[2], tile);
            var facil = GameManager.Facility.CreateFacility("Mineral_Rock", info);
        }
    }
    #endregion

    #region SLA - 스프라이트 에셋 생성(얘네는 저장이 안되서 매번 불러줘야함)

    public void Init_Statue_Sprite()
    {
        SLA_ObjectManager.Instance.CreateObject("Statue_Mana", new Vector3(-3.5f, -15f, 0));
        SLA_ObjectManager.Instance.SetLabel("Statue_Mana", "Mana", "Entry");

        SLA_ObjectManager.Instance.CreateObject("Statue_Gold", new Vector3(-3.5f, -13f, 0));
        SLA_ObjectManager.Instance.SetLabel("Statue_Gold", "Gold", "Entry");


        if (UserData.Instance.FileConfig.Statue_Dog)
        {
            SLA_ObjectManager.Instance.CreateObject("Statue_Dog", new Vector3(-5f, -13f, 0));
            SLA_ObjectManager.Instance.SetLabel("Statue_Dog", "Dog", "Entry");
        }

        if (UserData.Instance.FileConfig.Statue_Dragon)
        {
            SLA_ObjectManager.Instance.CreateObject("Statue_Dragon", new Vector3(-5f, -15f, 0));
            SLA_ObjectManager.Instance.SetLabel("Statue_Dragon", "Dragon", "Entry");
        }
    }

    #endregion




    #region 각 Floor의 초기화 설정

    void Init_EggFloor()
    {
        Init_Egg();
        Init_EggEntrance();
        Init_Statue();
    }

    void Init_2Floor()
    {

    }




    void Init_3Floor()
    {
        {        
            //? 1번 - 왼쪽 상단
            var main = CreateObj(2, new Vector2Int(0, 7), "RemoveableObstacle", CreateOption.Return) as RemoveableObstacle;
            var sub1 = CreateObj(2, new Vector2Int(1, 7), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub2 = CreateObj(2, new Vector2Int(2, 7), "Clone_Facility", CreateOption.Return) as Clone_Facility;

            main.Set_ObstacleType(RemoveableObstacle.Obj_Label.RO_F3_01);
            sub1.OriginalTarget = main;
            sub2.OriginalTarget = main;
        }

        {
            //? 2번 - 중앙
            var main = CreateObj(2, new Vector2Int(4, 2), "RemoveableObstacle", CreateOption.Return) as RemoveableObstacle;
            var sub1 = CreateObj(2, new Vector2Int(4, 3), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub2 = CreateObj(2, new Vector2Int(5, 2), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub3 = CreateObj(2, new Vector2Int(5, 3), "Clone_Facility", CreateOption.Return) as Clone_Facility;

            main.Set_ObstacleType(RemoveableObstacle.Obj_Label.RO_F3_02);
            sub1.OriginalTarget = main;
            sub2.OriginalTarget = main;
            sub3.OriginalTarget = main;
        }

        {
            //? 3번 - 하단 중앙
            var main = CreateObj(2, new Vector2Int(9, 0), "RemoveableObstacle", CreateOption.Return) as RemoveableObstacle;
            var sub1 = CreateObj(2, new Vector2Int(9, 1), "Clone_Facility", CreateOption.Return) as Clone_Facility;

            main.Set_ObstacleType(RemoveableObstacle.Obj_Label.RO_F3_03);
            sub1.OriginalTarget = main;
        }

        {
            //? 4번 - 오른쪽위
            var main = CreateObj(2, new Vector2Int(12, 11), "RemoveableObstacle", CreateOption.Return) as RemoveableObstacle;
            var sub1 = CreateObj(2, new Vector2Int(13, 11), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub2 = CreateObj(2, new Vector2Int(12, 12), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub3 = CreateObj(2, new Vector2Int(13, 12), "Clone_Facility", CreateOption.Return) as Clone_Facility;

            main.Set_ObstacleType(RemoveableObstacle.Obj_Label.RO_F3_04);
            sub1.OriginalTarget = main;
            sub2.OriginalTarget = main;
            sub3.OriginalTarget = main;
        }

        {
            //? 5번 - 오른쪽중앙
            var main = CreateObj(2, new Vector2Int(12, 4), "RemoveableObstacle", CreateOption.Return) as RemoveableObstacle;
            var sub1 = CreateObj(2, new Vector2Int(13, 4), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub2 = CreateObj(2, new Vector2Int(12, 5), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub3 = CreateObj(2, new Vector2Int(13, 5), "Clone_Facility", CreateOption.Return) as Clone_Facility;

            main.Set_ObstacleType(RemoveableObstacle.Obj_Label.RO_F3_05);
            sub1.OriginalTarget = main;
            sub2.OriginalTarget = main;
            sub3.OriginalTarget = main;
        }

        {
            //? 6번 - 오른쪽하단 야자수
            var main = CreateObj(2, new Vector2Int(14, 0), "RemoveableObstacle", CreateOption.Return) as RemoveableObstacle;
            var sub1 = CreateObj(2, new Vector2Int(14, 1), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub2 = CreateObj(2, new Vector2Int(14, 2), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub3 = CreateObj(2, new Vector2Int(15, 0), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub4 = CreateObj(2, new Vector2Int(15, 1), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub5 = CreateObj(2, new Vector2Int(15, 2), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub6 = CreateObj(2, new Vector2Int(16, 0), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub7 = CreateObj(2, new Vector2Int(16, 1), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub8 = CreateObj(2, new Vector2Int(16, 2), "Clone_Facility", CreateOption.Return) as Clone_Facility;

            main.Set_ObstacleType(RemoveableObstacle.Obj_Label.RO_F3_06);
            sub1.OriginalTarget = main;
            sub2.OriginalTarget = main;
            sub3.OriginalTarget = main;
            sub4.OriginalTarget = main;
            sub5.OriginalTarget = main;
            sub6.OriginalTarget = main;
            sub7.OriginalTarget = main;
            sub8.OriginalTarget = main;
        }
    }

    void Init_4Floor()
    {

    }




    public void Init_Egg()        //? 알 생성
    {
        var egg = CreateObj_OnlyOne(3, new Vector2Int(0, 2), "Egg_Lv1");
        Main.Instance.eggObj = egg.GetObject();

        Init_Obstacle();
    }

    void Init_Obstacle() //? 플레이어와 알 사이에 2칸도 그냥 설치불가능 지역으로 만들기
    {
        CreateObj(3, new Vector2Int(1, 2), "Obstacle", CreateOption.Return);
        CreateObj(3, new Vector2Int(2, 2), "Obstacle", CreateOption.Return);
    }


    void Init_EggEntrance()
    {
        CreateObj(3, new Vector2Int(12, 2), "Obstacle", CreateOption.Return);
        CreateObj(2, new Vector2Int(0, 0), "Obstacle", CreateOption.Return);
        CreateObj(4, new Vector2Int(1, 15), "Obstacle", CreateOption.Return);
    }

    void Init_Statue()
    {
        //? 골드 스태츄
        CreateObj(3, new Vector2Int(10, 4), "Statue_Gold", CreateOption.Return);
        CreateObj(3, new Vector2Int(10, 5), "Statue_Gold", CreateOption.Return);
        CreateObj(3, new Vector2Int(11, 4), "Statue_Gold", CreateOption.Return);
        CreateObj(3, new Vector2Int(11, 5), "Statue_Gold", CreateOption.Return);


        //? 마나 스태츄
        CreateObj(3, new Vector2Int(10, 0), "Statue_Mana", CreateOption.Return);
        CreateObj(3, new Vector2Int(10, 1), "Statue_Mana", CreateOption.Return);
        CreateObj(3, new Vector2Int(11, 0), "Statue_Mana", CreateOption.Return);
        CreateObj(3, new Vector2Int(11, 1), "Statue_Mana", CreateOption.Return);

        //? 개 스태츄
        if (UserData.Instance.FileConfig.Statue_Dog)
        {
            CreateObj(3, new Vector2Int(7, 4), "Statue_Dog", CreateOption.Replace);
            CreateObj(3, new Vector2Int(7, 5), "Statue_Dog", CreateOption.Replace);
            CreateObj(3, new Vector2Int(8, 4), "Statue_Dog", CreateOption.Replace);
            CreateObj(3, new Vector2Int(8, 5), "Statue_Dog", CreateOption.Replace);
        }
        else
        {
            CreateObj(3, new Vector2Int(7, 4), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(3, new Vector2Int(7, 5), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(3, new Vector2Int(8, 4), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(3, new Vector2Int(8, 5), "Obstacle_Wall", CreateOption.Replace);
        }

        //? 드래곤 스태츄
        if (UserData.Instance.FileConfig.Statue_Dragon)
        {
            CreateObj(3, new Vector2Int(7, 0), "Statue_Dragon", CreateOption.Replace);
            CreateObj(3, new Vector2Int(7, 1), "Statue_Dragon", CreateOption.Replace);
            CreateObj(3, new Vector2Int(8, 0), "Statue_Dragon", CreateOption.Replace);
            CreateObj(3, new Vector2Int(8, 1), "Statue_Dragon", CreateOption.Replace);
        }
        else
        {
            CreateObj(3, new Vector2Int(7, 0), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(3, new Vector2Int(7, 1), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(3, new Vector2Int(8, 0), "Obstacle_Wall", CreateOption.Replace);
            CreateObj(3, new Vector2Int(8, 1), "Obstacle_Wall", CreateOption.Replace);
        }

        //? 3번째 조각상 자리
        CreateObj(3, new Vector2Int(4, 0), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(4, 1), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(5, 0), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(5, 1), "Obstacle_Wall", CreateOption.Replace);

        //? 4번째 조각상 자리
        CreateObj(3, new Vector2Int(4, 4), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(4, 5), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(5, 4), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(5, 5), "Obstacle_Wall", CreateOption.Replace);


        Init_Statue_Sprite();
    }






    #endregion
}
