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

        //? ���� �������� ���� �̹� �۽Ǹ�Ƽ�� �����Ѵٸ�
        if (info.Place_Tile.Original != null && info.Place_Tile.Original as Facility != null) 
        {
            switch (createOption)
            {
                case CreateOption.Create: //? �׳� ���
                    break;

                case CreateOption.Replace: //? �ϴ� ������ ����
                    GameManager.Facility.RemoveFacility(info.Place_Tile.Original as Facility);
                    break;

                case CreateOption.Return: //? �������� �ٷ� ����
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




    #region Ŀ���� ������ ���ʽ�
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

    #region SLA - ��������Ʈ ���� ����(��״� ������ �ȵǼ� �Ź� �ҷ������)

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




    #region �� Floor�� �ʱ�ȭ ����

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
            //? 1�� - ���� ���
            var main = CreateObj(2, new Vector2Int(0, 7), "RemoveableObstacle", CreateOption.Return) as RemoveableObstacle;
            var sub1 = CreateObj(2, new Vector2Int(1, 7), "Clone_Facility", CreateOption.Return) as Clone_Facility;
            var sub2 = CreateObj(2, new Vector2Int(2, 7), "Clone_Facility", CreateOption.Return) as Clone_Facility;

            main.Set_ObstacleType(RemoveableObstacle.Obj_Label.RO_F3_01);
            sub1.OriginalTarget = main;
            sub2.OriginalTarget = main;
        }

        {
            //? 2�� - �߾�
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
            //? 3�� - �ϴ� �߾�
            var main = CreateObj(2, new Vector2Int(9, 0), "RemoveableObstacle", CreateOption.Return) as RemoveableObstacle;
            var sub1 = CreateObj(2, new Vector2Int(9, 1), "Clone_Facility", CreateOption.Return) as Clone_Facility;

            main.Set_ObstacleType(RemoveableObstacle.Obj_Label.RO_F3_03);
            sub1.OriginalTarget = main;
        }

        {
            //? 4�� - ��������
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
            //? 5�� - �������߾�
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
            //? 6�� - �������ϴ� ���ڼ�
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




    public void Init_Egg()        //? �� ����
    {
        var egg = CreateObj_OnlyOne(3, new Vector2Int(0, 2), "Egg_Lv1");
        Main.Instance.eggObj = egg.GetObject();

        Init_Obstacle();
    }

    void Init_Obstacle() //? �÷��̾�� �� ���̿� 2ĭ�� �׳� ��ġ�Ұ��� �������� �����
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
        //? ��� ������
        CreateObj(3, new Vector2Int(10, 4), "Statue_Gold", CreateOption.Return);
        CreateObj(3, new Vector2Int(10, 5), "Statue_Gold", CreateOption.Return);
        CreateObj(3, new Vector2Int(11, 4), "Statue_Gold", CreateOption.Return);
        CreateObj(3, new Vector2Int(11, 5), "Statue_Gold", CreateOption.Return);


        //? ���� ������
        CreateObj(3, new Vector2Int(10, 0), "Statue_Mana", CreateOption.Return);
        CreateObj(3, new Vector2Int(10, 1), "Statue_Mana", CreateOption.Return);
        CreateObj(3, new Vector2Int(11, 0), "Statue_Mana", CreateOption.Return);
        CreateObj(3, new Vector2Int(11, 1), "Statue_Mana", CreateOption.Return);

        //? �� ������
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

        //? �巡�� ������
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

        //? 3��° ������ �ڸ�
        CreateObj(3, new Vector2Int(4, 0), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(4, 1), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(5, 0), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(5, 1), "Obstacle_Wall", CreateOption.Replace);

        //? 4��° ������ �ڸ�
        CreateObj(3, new Vector2Int(4, 4), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(4, 5), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(5, 4), "Obstacle_Wall", CreateOption.Replace);
        CreateObj(3, new Vector2Int(5, 5), "Obstacle_Wall", CreateOption.Replace);


        Init_Statue_Sprite();
    }






    #endregion
}
