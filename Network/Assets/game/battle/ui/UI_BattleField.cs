using common.game.battle.engine.shape;
using game.battle.view;
using System;
using UnityEngine;
using UnityEngine.UI;
namespace game.battle.ui
{
    /// <summary>
    /// 战场ui类,控制摇杆及按键输入
    /// </summary>
    public class UI_BattleField : MonoBehaviour
    {
        Button crtBtn;
        Button endBtn;
        ETCJoystick MoveJoyStick;
        Transform myTransform;
        BattleField battleField;
        float joyPositionX = 0f;
        float joyPositionY = 0f;
        private bool isWorking;
        void Start()
        {
            myTransform = transform;
            crtBtn = myTransform.Find("CrtBtn").GetComponent<Button>();
            crtBtn.onClick.AddListener(OnCrtBtnClick);
            endBtn = myTransform.Find("EndBtn").GetComponent<Button>();
            endBtn.onClick.AddListener(OnEndBtnClick);
            MoveJoyStick = myTransform.Find("MoveJoystick").GetComponent<ETCJoystick>();
            MoveJoyStick.onMoveStart.AddListener(OnMoveStart);
            MoveJoyStick.onMove.AddListener(OnMove);
            MoveJoyStick.onMoveEnd.AddListener(OnMoveEnd);
            battleField = new PveBattleField();
            battleField.Init();
            battleField.Start();
            isWorking = true;
        }
        void OnEndBtnClick()
        {
            battleField.Stop();
            isWorking = false;
        }
        void OnCrtBtnClick()
        {
            if (battleField.Contrller != null)
                return;
            Shap shap = new Shap(10f, 10f, 10f);
            Vector3 position = new Vector3(89.5f, 0f, 134f);
            int unitCid = 1;
            battleField.CrtUnit(unitCid,true,shap, position);
        }
        void OnMoveStart()
        {
        }
        void OnMove(Vector2 call)
        {
            Unit controller = battleField.Contrller;
            if (controller == null)
                return;
            if (joyPositionX != call.x || joyPositionY != call.y)
            {
                //设置角色的朝向（朝向当前坐标+摇杆偏移量）  
                var playerPos = controller.Model.transform.localPosition;
                controller.Model.transform.LookAt(new Vector3(playerPos.x + joyPositionX, playerPos.y, playerPos.z + joyPositionY));
                controller.GetMove().Change(new common.game.battle.engine.Vector3(call.x,call.y,0), 10);
                //移动玩家的位置（按朝向位置移动）  
                //player.transform.Translate(Vector3.forward * Time.deltaTime * speed);
                //if (anim.GetBool("isAttack"))
                //{
                //    return;
                //}
                //anim.SetBool("isMove", true);
            }
        }
        void OnMoveEnd()
        {
            Unit controller = battleField.Contrller;
            controller.GetMove().Change(new common.game.battle.engine.Vector3(0, 0, 0).Normalize(), 0);
            //anim.SetBool("isMove", false);
        }

        void Update()
        {
        }
        void FixedUpdate()
        {
            if (!isWorking)
                return;
            DateTime runtime = DateTime.Now;
            battleField.Update(runtime);
        }
    }
}