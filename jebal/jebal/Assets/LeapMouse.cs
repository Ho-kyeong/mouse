using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Leap;
using Leap.Unity;

public class LeapMouse : MonoBehaviour
{
    Controller controller;
    Frame frame;

    float HandPalmPitch;
    float HandPalmYaw;
    float HandPalmRoll;
    float HandWristRot;

    bool indexPalmMove;
    bool closedFist;
    bool extendedThumb;
    bool ILYgesture;
    bool cali_flag;



    bool indexMove;
    bool indexCali;
    bool indexCaliPlus;
    bool indexCaliMinus;
    bool indexCaliCondition1;
    bool indexCaliCondition2;

    // for calibration
    int rangeX;
    int rangeY;
    float palm_x_max;
    float palm_x_min;

    float palm_y_max;
    float palm_y_min;

    float palm_x_now;
    float palm_y_now;

    float x_ratio;
    float y_ratio;
    float idx_speed;
    float idy_speed;
    float idx_ratio;
    float idy_ratio;

    int y_cali;

    int currentResolutionWidth; //현재 스크린 너비
    int currentResolutionHeight;  // 현재 스크린의 높이
    int currentResolutionCenterWidth;
    int currentResolutionCenterHeight;

    Vector2 mousePosition;

    int prePosX;
    int prePosY;

    //Bring object Img
    GameObject thumb;
    GameObject index;
    GameObject twoFinger;
    GameObject threeFinger;
    GameObject fourFinger;
    GameObject hand;
    GameObject grab;
    GameObject L;
    GameObject ILY;
    GameObject def;
    GameObject mouseMove;




    // for mouse control
    [DllImport("user32")]
    static extern bool SetCursorPos(int X, int Y);  //현재 커서 가지고 오는 거.
    [DllImport("User32")]
    static extern void mouse_event(ulong flag, int X, int Y, int dwData); //mouse로 event주는애.

    //keyboard control
    [DllImport("User32")]
    static extern void keybd_event(uint vk, uint scan, uint flags, uint extraInfo);

    //Build size setting
    [DllImport("user32.dll")]
    private static extern int GetActiveWindow();
    [DllImport("user32.dll")]
    private static extern long GetWindowLong(int hwnd, int nIndex);
    [DllImport("user32.dll")]
    static extern int SetWindowLong(int hwnd, int nIndex, long dwNewLong);
    [DllImport("user32.dll")]
    public static extern bool SetWindowText(int hwnd, System.String lpString);

    //좌클릭
    ulong MOUSEEVENTF_LEFTDOWN = 0x0002;
    ulong MOUSEEVENTF_LEFTUP = 0x0004;
    //우클릭
    ulong MOUSEEVENTF_RIGHTDOWN = 0x0008;
    ulong MOUSEEVENTF_RIGHTUP = 0x0010;
    //마우스휠
    ulong MOUSEEVENTF_WHEEL = 0x800;

    //rotate 변수
    bool fingerRotateR1;
    bool fingerRotateR2;
    bool fingerRotateR3;

    bool fingerRotateL1;
    bool fingerRotateL2;
    bool fingerRotateL3;

    bool backPage1;
    bool backPage2;
    bool forwardPage1;
    bool forwardPage2;
    
    bool secondHandClosedFist1;
    bool secondHandClosedFist2;


    void Start()
    {
        controller = new Controller();

        //Screen size setting
        Screen.SetResolution(80 * 16 / 10, 80, false);
        int handle = GetActiveWindow();
        long lCurStyle = GetWindowLong(handle, -16);
        lCurStyle &= ~524288;
        SetWindowLong(handle, -16, lCurStyle); //시스템메뉴 삭제

        //mouse event
        controller.Config.SetFloat("Gesture.KeyTap.MinDownVelocity", 40.0f);
        controller.Config.SetFloat("Gesture.KeyTap.HistorySeconds", .2f);
        controller.Config.SetFloat("Gesture.KeyTap.MinDistance", 1.0f);
        controller.Config.Save();

        //initialization x,y
        currentResolutionWidth = Screen.currentResolution.width;  //해상도 x
        currentResolutionHeight = Screen.currentResolution.height;  // 해상도y
        currentResolutionCenterWidth = currentResolutionWidth / 2;
        currentResolutionCenterHeight = currentResolutionHeight / 2;
        prePosX = currentResolutionCenterWidth;
        prePosY = currentResolutionCenterHeight;

        //association cali
        palm_x_min = 0.0f;
        palm_y_max = 0.0f;
        palm_y_min = currentResolutionCenterHeight;

        //mouse moving value
        x_ratio = 4.0f;
        y_ratio = 3.0f;
        cali_flag = false;        

        //rotate
        fingerRotateR1 = false;
        fingerRotateR2 = false;
        fingerRotateR3 = false;

        fingerRotateL1 = false;
        fingerRotateL2 = false;
        fingerRotateL3 = false;

        backPage1 = false;
        backPage2 = false;
        forwardPage1 = false;
        forwardPage2 = false;

        secondHandClosedFist1 = false;
        secondHandClosedFist2 = false;

        //Making the img
        thumb = GameObject.Find("thumb") as GameObject;
        index = GameObject.Find("index") as GameObject;
        twoFinger = GameObject.Find("twoFinger") as GameObject;
        threeFinger = GameObject.Find("threeFinger") as GameObject;
        fourFinger = GameObject.Find("fourFinger") as GameObject;
        hand = GameObject.Find("hand") as GameObject;
        grab = GameObject.Find("grab") as GameObject;
        L = GameObject.Find("L") as GameObject;
        ILY = GameObject.Find("ILY") as GameObject;
        def = GameObject.Find("def") as GameObject;
        mouseMove = GameObject.Find("MouseMove") as GameObject;

        idx_speed = 0.0f;
        idy_speed = 0.0f;

        indexCaliPlus = false;
        indexCaliMinus = false;
        idx_ratio = 1.0f;
        idy_ratio = 1.0f;
        
    }


    // Update is called once per frame
    void Update()
    {
        controller = new Controller();
        Frame frame = controller.Frame();
        List<Hand> hands = frame.Hands;
        Hand firstHand = null;
        Hand secondHand = null;


        if (frame.Hands.Count > 0)
        {
            firstHand = hands[0];

            if (frame.Hands.Count > 1)
            {
                secondHand = hands[1];
            }
        }

        Vector position = hands[0].PalmPosition;

        Vector thumbDir = firstHand.Fingers[0].Direction;
        Vector indexDir = firstHand.Fingers[1].Direction;
        Vector thirdDir = firstHand.Fingers[2].Direction;
        Vector fourthDir = firstHand.Fingers[3].Direction;
        Vector babyDir = firstHand.Fingers[4].Direction;
        //Vector thumbPos = firstHand.Fingers[0].TipPosition;
        //Vector indexPos = firstHand.Fingers[1].TipPosition;
        //Vector thirdPos = firstHand.Fingers[2].TipPosition;
        //Vector fourthPos = firstHand.Fingers[3].TipPosition;
        //Vector babyPos = firstHand.Fingers[4].TipPosition;

        //Debug.Log(thumbDir);
        //Debug.Log(indexDir);
        //Debug.Log(thirdDir);
        //Debug.Log(fourthDir);
        //Debug.Log(babyDir);


        //scroll 제어
        if (thumbDir.y > 0.65 && indexDir.y > 0.65 && thirdDir.y > 0.65 && fourthDir.y > 0.65 && babyDir.y > 0.65)
        {
            //마우스 휠 up
            mouse_event(MOUSEEVENTF_WHEEL, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 60);
        }
        else if (thumbDir.y < -0.45 && indexDir.y < -0.55 && thirdDir.y < -0.55 && fourthDir.y < -0.55 && babyDir.y < -0.45)
        {
            //마우스 휠 down
            mouse_event(MOUSEEVENTF_WHEEL, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), -60);
        }

        //뒤로가기 앞으로가기
        if ((thumbDir.x > 0.15 && indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.15) | backPage1 == true)
        {
            //Debug.Log("1");
            backPage1 = true;
            if ((thumbDir.x < -0.1 && indexDir.x < -0.20 && thirdDir.x < -0.20 && fourthDir.x < -0.20 && babyDir.x < -0.1) | backPage2 == true)
            {
                //Debug.Log("2");
                backPage2 = true;
                if (thumbDir.x < -0.6 && indexDir.x < -0.65 && thirdDir.x < -0.65 && fourthDir.x < -0.65 && babyDir.x < -0.6)
                {
                    //Debug.Log("3");
                    keybd_event(0x12, 0, 0x00, 0);
                    keybd_event(0x25, 0, 0x00, 0);
                    keybd_event(0x25, 0, 0x02, 0);
                    keybd_event(0x12, 0, 0x02, 0);
                    backPage1 = false;
                    backPage2 = false;
                }
                else if (thumbDir.x > 0 && indexDir.x > 0 && thirdDir.x > 0 && fourthDir.x > 0 && babyDir.x > 0)
                {
                    backPage1 = false;
                    backPage2 = false;
                }
            }

        }

        if ((thumbDir.x < -0.15 && indexDir.x < -0.2 && thirdDir.x < -0.2 && fourthDir.x < -0.2 && babyDir.x < -0.15) | forwardPage1 == true)
        {
            forwardPage1 = true;
            //Debug.Log("4");
            if ((thumbDir.x > 0.1 && indexDir.x > 0.20 && thirdDir.x > 0.20 && fourthDir.x > 0.20 && babyDir.x > 0.1) | forwardPage2 == true)
            {
                forwardPage2 = true;
                //Debug.Log("5");
                if (thumbDir.x > 0.35 && indexDir.x > 0.45 && thirdDir.x > 0.45 && fourthDir.x > 0.45 && babyDir.x > 0.35)
                {
                    //Debug.Log("6");
                    keybd_event(0x12, 0, 0x00, 0);
                    keybd_event(0x27, 0, 0x00, 0);
                    keybd_event(0x27, 0, 0x02, 0);
                    keybd_event(0x12, 0, 0x02, 0);
                    forwardPage1 = false;
                    forwardPage2 = false;
                }
                else if (thumbDir.x < 0 && indexDir.x < 0 && thirdDir.x < 0 && fourthDir.x < 0 && babyDir.x < 0)
                {
                    forwardPage1 = false;
                    forwardPage2 = false;
                }
            }


        }
        else
        {
            backPage1 = false;
            backPage2 = false;
            forwardPage1 = false;
            forwardPage2 = false;
        }

        //using rotation
        //clock rotate
        if ((indexDir.x > 0.15 && indexDir.y < -0.15) | fingerRotateR1 == true)
        {
            fingerRotateR1 = true;
            //Debug.Log("2");

            if ((indexDir.x < -0.15 && indexDir.y < -0.15) | fingerRotateR2 == true)
            {
                fingerRotateR2 = true;
                //Debug.Log("4");

                if ((indexDir.x < -0.15 && indexDir.y > 0.15) | fingerRotateR3 == true)
                {
                    fingerRotateR3 = true;
                    //Debug.Log("6");

                    if (indexDir.x > 0.15 && indexDir.y > 0.15)
                    {
                        //Event.KeyboardEvent("k");
                        keybd_event(0x4A, 0, 0x00, 0);
                        keybd_event(0x4A, 0, 0x02, 0);
                        //Debug.Log("explore");
                        fingerRotateR1 = false;
                        fingerRotateR2 = false;
                        fingerRotateR3 = false;
                    }
                    else if (indexDir.x < -0.15 && indexDir.y < -0.15)
                    {
                        fingerRotateR1 = false;
                        fingerRotateR2 = false;
                        fingerRotateR3 = false;
                    }
                }
                else if (indexDir.x > 0.15 && indexDir.y < -0.15)
                {
                    fingerRotateR1 = false;
                    fingerRotateR2 = false;
                }
            }
            else if (indexDir.x > 0.15 && indexDir.y > 0.15)
            {
                fingerRotateR1 = false;
            }
        }

        //counter clock rotate
        if ((indexDir.x < -0.15 && indexDir.y < -0.15) | fingerRotateL1 == true)
        {
            fingerRotateL1 = true;
            //Debug.Log("2");

            if ((indexDir.x > 0.15 && indexDir.y < -0.15) | fingerRotateL2 == true)
            {
                fingerRotateL2 = true;
                //Debug.Log("4");

                if ((indexDir.x > 0.15 && indexDir.y > 0.15) | fingerRotateL3 == true)
                {
                    fingerRotateL3 = true;
                    //Debug.Log("6");

                    if (indexDir.x < -0.15 && indexDir.y > 0.15)
                    {
                        //Event.KeyboardEvent("k");
                        keybd_event(0x4C, 0, 0x00, 0);
                        keybd_event(0x4C, 0, 0x02, 0);
                        //Debug.Log("explore");
                        fingerRotateL1 = false;
                        fingerRotateL2 = false;
                        fingerRotateL3 = false;

                    }
                    else if (indexDir.x > 0.15 && indexDir.y < -0.15)
                    {
                        fingerRotateL1 = false;
                        fingerRotateL2 = false;
                        fingerRotateL3 = false;
                    }
                }
                else if (indexDir.x < -0.15 && indexDir.y < -0.15)
                {
                    fingerRotateL1 = false;
                    fingerRotateL2 = false;
                }
            }
            else if (indexDir.x < -0.15 && indexDir.y > 0.15)
            {
                fingerRotateL1 = false;
            }
        }


        //gestrue recognize

        // fist detection

        //mode change & index move cali
        //모드 체인지에서 인덱스 모션 아닐때 초기화 추가
        

        if (frame.Hands.Count > 1 && !firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended)
        {
            Finger secThumb = secondHand.Fingers[0];
            Finger secIndex = secondHand.Fingers[1];
            Finger secThird = secondHand.Fingers[2];
            Finger secFourth = secondHand.Fingers[3];
            Finger secBaby = secondHand.Fingers[4];
            //Debug.Log("1");
            if (((!secThumb.IsExtended && !secIndex.IsExtended && !secThird.IsExtended &&!secFourth.IsExtended && !secBaby.IsExtended) && (indexMove == true | indexPalmMove == true)) | secondHandClosedFist1 == true)
            {
                //Debug.Log("2");
                secondHandClosedFist1 = true;
                if ((secThumb.IsExtended && secIndex.IsExtended && secThird.IsExtended &&secFourth.IsExtended && secBaby.IsExtended) | secondHandClosedFist2 == true)
                {
                    //Debug.Log("3");
                    secondHandClosedFist2 = true;
                    if (!secThumb.IsExtended && !secIndex.IsExtended && !secThird.IsExtended &&!secFourth.IsExtended && !secBaby.IsExtended)
                    {
                        System.Threading.Thread.Sleep(200);
                        //Debug.Log("44444444444444");
                        System.Threading.Thread.Sleep(200);
                        if (indexMove == true)
                        {
                            //Debug.Log("5");
                            indexMove = false;
                            indexPalmMove = true;
                            secondHandClosedFist1 = false;
                            secondHandClosedFist2 = false;
                        }
                        else if (indexPalmMove == true)
                        {
                            //Debug.Log("6");
                            indexPalmMove = false;
                            indexMove = true;
                            secondHandClosedFist1 = false;
                            secondHandClosedFist2 = false;
                        }
                    }

                }
            }

            //if ((secThumb.IsExtended && secIndex.IsExtended && secThird.IsExtended && secFourth.IsExtended && secBaby.IsExtended && secThumb.Direction.y>0.55
            //    && secIndex.Direction.y>0.75 && secThird.Direction.y>0.75 && secFourth.Direction.y>0.75 && secBaby.Direction.y>0.65) | indexCaliCondition1)
            //{
            //    indexCaliCondition1 = true;

            //}
            
        }
        else
        {
            secondHandClosedFist1 = false;
            secondHandClosedFist2 = false;
        }

        //Debug.Log("palm"+indexPalmMove);
        //Debug.Log("index"+indexMove);

        if (!firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended)
        {
            if (indexMove == false && indexPalmMove == false)
            {
                indexMove = true;
            }
            //if(indexPalmMove == false)
            //{
            //    indexMove = true; // plain mouse 추후에 설정
            //}
            else if(indexMove == false)
            {
                indexPalmMove = true;
            }

            if (Input.GetKey(KeyCode.W))
            {
                indexCaliPlus = true;
                System.Threading.Thread.Sleep(100);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                indexCaliMinus = true;
                System.Threading.Thread.Sleep(100);
            }

        }

        //index Mode
        //if (!firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended)
        //{
        //    appear(thumb);
        //    indexMove = true;
        //}
        /* else if (firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && !firstHand.Fingers[4].IsExtended)
         {
             closedFist = false;
             mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);  // Screen, center callibration
             Debug.Log("현재 마우스 위치 : " + mousePosition);
         }*/

        
        else if (firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended && firstHand.Fingers[4].IsExtended)
        {
            ILYgesture = true;
            appear(ILY);
        }
        else if (indexMove == true && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && firstHand.Fingers[2].IsExtended && firstHand.Fingers[3].IsExtended
             && firstHand.Fingers[4].IsExtended)
        {
            indexCaliPlus = true; //손가락 방향 속도 캘리 속도 ++
        }
        else if (indexMove == true && firstHand.Fingers[0].IsExtended && firstHand.Fingers[1].IsExtended && !firstHand.Fingers[2].IsExtended && !firstHand.Fingers[3].IsExtended
            && firstHand.Fingers[4].IsExtended)
        {
            indexCaliMinus = true; //속도 --
        }

        //calibration하고 나서 나머지 값들 초기화
        else
        {
            y_cali = currentResolutionCenterHeight - 200;
            closedFist = false;
            extendedThumb = false;
            ILYgesture = false;
            indexCali = false;
            indexMove = false;
            indexPalmMove = false;
        }

        //index move calibration 속도 조절

        if (indexCaliPlus == true && idx_ratio < 8.0f && idy_ratio < 8.0f)
        {
            idx_ratio = idx_ratio + 0.5f;
            idy_ratio = idy_ratio + 0.5f;
            indexCaliPlus = false;
            Debug.Log("up");
        }
        else if (indexCaliMinus == true && idx_ratio > 0.8f && idy_ratio > 0.8f)
        {
            idx_ratio = idx_ratio - 0.5f;
            idy_ratio = idy_ratio - 0.5f;
            indexCaliMinus = false;
            Debug.Log("down");
        }

        //palm move calibration중
        if (ILYgesture == true)
        {
            appear(ILY);
            // x range calculation , x_ max=o.of ,x_min=해상도
            palm_x_now = position.x;
            if (palm_x_max < palm_x_now)
            {   // 최대< 현재
                palm_x_max = palm_x_now;    //최대 <- 현재
                //Debug.Log("Big Size ");
            }
            else if (palm_x_min > palm_x_now)
            {
                palm_x_min = palm_x_now;
                //Debug.Log("Small Size ");
            }

            // y range calculation
            palm_y_now = position.y;
            if (palm_y_max < palm_y_now)
            {
                palm_y_max = palm_y_now;
            }
            else if (palm_y_min > palm_y_now)
            {
                palm_y_min = palm_y_now;
            }

            //Debug.Log("palm_x_max: " + palm_x_max);
            //Debug.Log("palm_x_min: " + palm_x_min);
            //Debug.Log("palm_y_max: " + palm_y_max);
            //Debug.Log("palm_y_min: " + palm_y_min);
            // x, y range
            float x_leap = palm_x_max - palm_x_min;
            float y_leap = palm_y_max - palm_y_min;

            //Debug.Log("x_leap: " + x_leap);
            //Debug.Log("y_leap: " + y_leap);

            // x, y ratio
            x_ratio = currentResolutionWidth / x_leap;
            y_ratio = currentResolutionHeight / y_leap;

            cali_flag = true;
        }
        //Mouse Control
        else if (indexPalmMove == true)
        {
            if (cali_flag == true)
            {
                y_cali = currentResolutionCenterHeight + 200;
            }

            //move
            SetCursorPos(currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y)); // this!

            Vector velocity = hands[0].Fingers[1].TipVelocity;
            float speed = velocity.Magnitude;
            //Debug.Log("tip speed " + speed);

            Vector handSpeed = hands[0].PalmVelocity;
            //Debug.Log("hands speed " + handSpeed);
            if (speed > 1000 && handSpeed[0] < 300 && handSpeed[1] < 300 && handSpeed[2] < 200 && handSpeed[0] > -300 && handSpeed[1] > -300 && handSpeed[2] > -200)
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                System.Threading.Thread.Sleep(100);
                mouse_event(MOUSEEVENTF_LEFTUP, currentResolutionCenterWidth + (int)(x_ratio * position.x), currentResolutionHeight + y_cali - (int)(y_ratio * position.y), 0);
                System.Threading.Thread.Sleep(150);

            }

        }

        //IDXMODE
        else if (indexMove) //여기다 넣어요
        {
            Vector direction = hands[0].Fingers[1].Direction;
            //Debug.Log("direction " + direction);

            Vector tipPosition = hands[0].Fingers[1].TipPosition;
            //Debug.Log("postion " + tipPosition);

            Vector tipPosition2 = hands[0].Fingers[2].TipPosition;
            

            SetCursorPos(currentResolutionCenterWidth + (int)xSetting(firstHand), currentResolutionCenterHeight + (int)ySetting(firstHand));
            //Debug.Log("tipPosition" +tipPosition.z);

             if (tipPosition.z < -100 && tipPosition2.z < -100 && hands[0].Fingers[2].IsExtended)
            {
                //Debug.Log("Index RClick");
                appear(twoFinger);
                mouse_event(MOUSEEVENTF_RIGHTDOWN, (currentResolutionCenterWidth + (int)xSetting(firstHand)), currentResolutionCenterHeight + (int)ySetting(firstHand), 0);
                System.Threading.Thread.Sleep(100);
                mouse_event(MOUSEEVENTF_RIGHTUP, (currentResolutionCenterWidth + (int)xSetting(firstHand)), currentResolutionCenterHeight + (int)ySetting(firstHand), 0);
                System.Threading.Thread.Sleep(150);

            }

            else if (tipPosition.z < -100)
            {
                //Debug.Log("Index LClick");
                appear(L);
                mouse_event(MOUSEEVENTF_LEFTDOWN, (currentResolutionCenterWidth + (int)xSetting(firstHand)), currentResolutionCenterHeight + (int)ySetting(firstHand), 0);
                System.Threading.Thread.Sleep(100);
                mouse_event(MOUSEEVENTF_LEFTUP, (currentResolutionCenterWidth + (int)xSetting(firstHand)), currentResolutionCenterHeight + (int)ySetting(firstHand), 0);
                System.Threading.Thread.Sleep(150);
            }

          

        }
        


    }

    public float xSetting(Hand hand)
    {
        if (hand.Fingers[1].Direction.x < -0.25)
        {
            //감소하는 손가락 
            appear(index);
            idx_speed = idx_speed - (1*idx_ratio);
        }
        else if (hand.Fingers[1].Direction.x > 0.25)
        {
            //증가하는 손가락 
            appear(index);
            idx_speed = idx_speed + (1 * idx_ratio);
        }
        return idx_speed;
    }

    public float ySetting(Hand hand)
    {
        if (hand.Fingers[1].Direction.y < -0.25)
        {
            appear(index);
            idy_speed = idy_speed + (1 * idy_ratio);
        }
        else if (hand.Fingers[1].Direction.y > 0.25)
        {
            appear(index);
            idy_speed = idy_speed - (1 * idy_ratio);
        }
        return idy_speed;
    }

    
    public void appear(GameObject obj)
    {
        thumb.transform.position = new Vector3(0f, 0f, 2.0f);
        index.transform.position = new Vector3(0f, 0f, 2.0f);
        twoFinger.transform.position = new Vector3(0f, 0f, 2.0f);
        threeFinger.transform.position = new Vector3(0f, 0f, 2.0f);
        fourFinger.transform.position = new Vector3(0f, 0f, 2.0f);
        hand.transform.position = new Vector3(0f, 0f, 2.0f);
        grab.transform.position = new Vector3(0f, 0f, 2.0f);
        L.transform.position = new Vector3(0f, 0f, 2.0f);
        ILY.transform.position = new Vector3(0f, 0f, 2.0f);
        def.transform.position = new Vector3(0f, 0f, 2.0f);
        obj.transform.position = new Vector3(0f, 0f, 0.01f);
        //mouseMove.transform.position = new Vector3(0f, 0f, 2.0f);

    }


}
