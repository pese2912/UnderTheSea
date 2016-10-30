using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;



public class Arkanoid : EditorWindow
{
    PTData DB;

    [MenuItem("NEXTI/Ark!")]
    static void ToolBarWindow()
    {
        Arkanoid window = (Arkanoid)EditorWindow.GetWindow(typeof(Arkanoid));
        window.Show();
    }

    GUIStyle Ball = new GUIStyle();


    enum state
    {
        NONE = 0,
        INTRO,
        PLAYING,
        END
    }
    state STATE = state.NONE;


    bool loadSwitch;

    float padX;


    float speed;
    double xdir;
    double xdirValue;
    double ydir;
    double ballXpos;
    double ballYpos;

    double newballXpos;
    double newballYpos;

    float PaddlePosX;
    float paddleWidth;

    float scrollBgPosy;
    float scrollBgSpeed;

    int[,] stageBricks; // 브릭 1 , 0

    float paddleEffect;
    float paddleEffectSpeed;

    Vector2 pos = new Vector2(0, 0); // BRICK POS
    Vector2 scale; // BRICK SCALE

    //-- [ GUI STYLE ] --
    GUIStyle scoreLineStyle = new GUIStyle();
    GUIStyle scoreLineFontStyle = new GUIStyle();
    GUIStyle scrollBgStyle = new GUIStyle();
    GUIStyle footerStyle = new GUIStyle();
    GUIStyle brLineStyle = new GUIStyle();
    GUIStyle elementStyle = new GUIStyle();

    GUIStyle brickStyle = new GUIStyle();
    GUIStyle ballStyle = new GUIStyle();
    GUIStyle paddleStyle = new GUIStyle();
    GUIStyle playingFontStyle = new GUIStyle();
    GUIStyle playingStateStyle = new GUIStyle();

    GUIStyle paddleLigheStyle = new GUIStyle();

    int currentTimeSecond;

    int hightScore;
    int currentScore;
    int playerLife;
    int stage;
    string playTime;

    int brickRow;
    int brickCol;

    int currentStage;

    float plusYpos;

    string startTime;
    string currentTime;
    string calTime;
    bool timecalcu;

    int nextStageNum;

    void OnEnable()
    {
        //285 x 366
        this.maxSize = new Vector2(285,408);
        this.minSize = new Vector2(285, 408);

        LoadData();  //데이터 로드
        

        xdir = 0.7f;
        ydir = 0.7f;
        xdirValue = xdir;

        ballXpos = Random.Range(10,270);
        ballYpos = 100;
       
        paddleWidth = 64;
        
        STATE = state.INTRO;



        scoreLineStyle.normal.background = DB.DBTexture[86];
        scoreLineFontStyle.normal.background = null;
        scoreLineFontStyle.font = DB.DBFonts[2];
        scoreLineFontStyle.fontSize = 12;
        scoreLineFontStyle.normal.textColor = Corr(194,194,194,255);

        scrollBgStyle.normal.background = DB.DBTexture[83];
        footerStyle.normal.background = DB.DBTexture[87];

        ballStyle.normal.background = DB.DBTexture[90];
        paddleStyle.normal.background = DB.DBTexture[95];

        paddleLigheStyle.normal.background = DB.DBTexture[98];

        brickStyle.normal.background = DB.DBTexture[92];

        playingStateStyle.font = DB.DBFonts[2];
        playingStateStyle.normal.background = null;

        scrollBgPosy = 0;
        scrollBgSpeed = 0.1f;

        playerLife = 3;


        nextStageNum = 0;
        //Debug.Log(Screen.height - (89 + 9));
        //Debug.Log(Screen.height - 89);
        //Debug.Log(Screen.height - 85);
        plusYpos = 20;


        //paddle effect
        paddleEffect = 0;
        paddleEffectSpeed = 1f;
    }

 
    void Reset()
    {
        xdir = 0.5f;
        ydir = 0.5f;
    }


    void LoadData()
    {
        DB = (PTData)AssetDatabase.LoadAssetAtPath("Assets/POWERTOOLS/Data/ptDatas.asset", typeof(PTData));
    }

    
    void OnGUI()
    {
        TopScoreLine();

        if (STATE == state.INTRO) Intro();
        if (STATE == state.PLAYING) Playing();
        if (STATE == state.END) End();


        Footer();
        //DrawBricks();
        //Balls();
        //Paddle();

        // 경과시간 계산
        
        Repaint();
       
    }


    void TopScoreLine()
    {
        GUILayout.Box("", scoreLineStyle ,GUILayout.MinWidth(Screen.width), GUILayout.Height(20));
        // 인트로 상태일대
        if (STATE == state.INTRO)
        {
            scoreLineFontStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0,2,Screen.width,20), "HIGH SCORE : "+hightScore, scoreLineFontStyle);
        }

        // 플레이 상태일때.
        if(STATE == state.PLAYING)
        {
            scoreLineFontStyle.alignment = TextAnchor.MiddleLeft;
            GUI.Label(new Rect(4, 2, Screen.width, 20), "SCORE : " + currentScore, scoreLineFontStyle);
        }

        if(STATE == state.END)
        {
            if (hightScore < currentScore) hightScore = currentScore;
            scoreLineFontStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(4, 2, Screen.width, 20), "HIGH SCORE : " + hightScore, scoreLineFontStyle);
        }

        brLineStyle.normal.background = DB.DBTexture[85];
        GUILayout.Box("", brLineStyle, GUILayout.MinWidth(Screen.width), GUILayout.Height(1));
    }

  
  
    // 인트로 화면.
    void Intro()
    {
        
        if (scrollBgPosy < -372) scrollBgPosy = 0;
        scrollBgPosy -= scrollBgSpeed;
        GUILayout.BeginArea(new Rect(0, 21, 285, 372));
        GUI.Box(new Rect(0, scrollBgPosy, 285, 744), "", scrollBgStyle);

        //LOGO
        elementStyle.normal.background = DB.DBTexture[84];
        GUI.Box(new Rect(30, 60, 235, 58), "",elementStyle);
        GUILayout.EndArea();

        //play button
        currentTimeSecond = System.DateTime.Now.Second;
        if (currentTimeSecond % 2 == 0) elementStyle.normal.background = DB.DBTexture[88];
        else elementStyle.normal.background = DB.DBTexture[89];
        if(GUI.Button(new Rect(72, 335, 145, 19), "", elementStyle))
        {
            loadSwitch = true;

            StageSetting(2, 4);

            startTime = System.DateTime.Now.ToString("h:mm:ss.ff");

            timecalcu = true;
            STATE = state.PLAYING;
        }
        GUI.color = Corr(100, 100, 100, 30);
        GUI.Box(new Rect(72 + 4, 335 + 4, 145, 19), "", elementStyle);
        GUI.color = Color.white;

    }

    // 스테이지 셋팅
    void StageSetting(int row, int col)
    {
        if(loadSwitch == true)
        {
            scale = new Vector2(285.0f / col, 21);
            currentStage++;
            scrollBgSpeed = 0.05f;

            brickRow = row;
            brickCol = col;
            stageBricks = new int[brickRow, brickCol]; // 2,5
            BrickCreate();

           // nextStageNum = stageBricks.Length;
            loadSwitch = false;
        }
    }

    void Playing()
    {
        
        

        if (scrollBgPosy < -372) scrollBgPosy = 0;
        scrollBgPosy -= scrollBgSpeed;

        GUILayout.BeginArea(new Rect(0, 21, 285, 372));
        GUI.color = Corr(255, 255, 255, 200);
        GUI.Box(new Rect(0, scrollBgPosy, 285, 744), "", scrollBgStyle);
        GUI.color = Color.white;

        // state text
        PlayingState();


        DrawBricksShadwo(brickRow, brickCol);
        DrawBricks(brickRow, brickCol);
        Balls();



        Paddle();
        GUILayout.EndArea();



       // Debug.Log(stageBricks.Length);
    }

    // 경과 시간 계산
    void TimeCal()
    {
        if (timecalcu == true)
        { 
          currentTime = System.DateTime.Now.ToString("h:mm:ss.ff");
          System.DateTime start = System.Convert.ToDateTime(startTime);
          System.DateTime current = System.Convert.ToDateTime(currentTime);
          System.TimeSpan timeCal = current - start;
          calTime = timeCal.ToString();
        }
    }

    // 플레이 텍스트 상태.
    void PlayingState()
    {
        TimeCal();

        playingStateStyle.normal.textColor = Corr(150, 150, 150, 120);
        playingStateStyle.alignment = TextAnchor.UpperCenter;

        
        playingStateStyle.fontSize = 28;
        GUI.Label(new Rect(0, 79, 285, 100), "STAGE", playingStateStyle);

        playingStateStyle.fontSize = 161;
        GUI.Label(new Rect(10, 116, 285, 150), ""+ currentStage, playingStateStyle);

        playingStateStyle.fontSize = 18;
        GUI.Label(new Rect(0, 268, 285, 50), "PLAY TIME", playingStateStyle);

        playingStateStyle.fontSize = 15;
        playingStateStyle.alignment = TextAnchor.UpperLeft;
        GUI.Label(new Rect(79, 287, 285, 50), calTime.Substring(0, calTime.Length - 5), playingStateStyle);

    }


    // END 텍스트 쉐도우
    void EndStateShadow()
    {
        playingStateStyle.normal.textColor = Corr(50, 50, 50, 30);
        playingStateStyle.alignment = TextAnchor.UpperCenter;


        playingStateStyle.fontSize = 22;
        GUI.Label(new Rect(0 + 5, 79+5, 285, 100), "YOUR SCORE", playingStateStyle);
        

        playingStateStyle.fontSize = 100;
        GUI.Label(new Rect(10 + 5,135 + 5, 285, 150), "" + currentScore, playingStateStyle);

        //playingStateStyle.fontSize = 18;
        //GUI.Label(new Rect(0 + 5, 268-10 + 5, 285, 50), "PLAY TIME", playingStateStyle);

        //playingStateStyle.fontSize = 15;
        //playingStateStyle.alignment = TextAnchor.UpperCenter;
        //GUI.Label(new Rect(0 + 5, 287-10 + 5, 285, 50), calTime.Substring(0, calTime.Length - 5), playingStateStyle);

    }

    // END 텍스트
    void EndState()
    {
        playingStateStyle.normal.textColor = Corr(50, 50, 50, 255);
        playingStateStyle.alignment = TextAnchor.UpperCenter;


        playingStateStyle.fontSize = 22;
        GUI.Label(new Rect(0, 79, 285, 100), "YOUR SCORE", playingStateStyle);

        playingStateStyle.fontSize = 100;
        GUI.Label(new Rect(10, 135, 285, 150), "" + currentScore, playingStateStyle);

        playingStateStyle.fontSize = 18;
        GUI.Label(new Rect(0, 268-10, 285, 50), "PLAY TIME", playingStateStyle);

        playingStateStyle.fontSize = 15;
        playingStateStyle.alignment = TextAnchor.UpperCenter;
        GUI.Label(new Rect(0, 287-10, 285, 50), calTime.Substring(0, calTime.Length - 8), playingStateStyle);

    }

    float repSpeed = 0.4f;
    float reptime = 255;

    void End()
    {
        if (scrollBgPosy < -372) scrollBgPosy = 0;
        scrollBgPosy -= scrollBgSpeed;
        try { 
        GUILayout.BeginArea(new Rect(0, 21, 285, 372));
        GUI.Box(new Rect(0, scrollBgPosy, 285, 744), "", scrollBgStyle);
        GUILayout.EndArea();
        }
        catch
        {
            //BeginArea ERROE
        }
        EndStateShadow();
        EndState();

        currentTimeSecond = System.DateTime.Now.Second;
        //if (currentTimeSecond % 2 == 0) elementStyle.normal.background = DB.DBTexture[88];
        //else elementStyle.normal.background = DB.DBTexture[89];

        playingStateStyle.fontSize = 20;
        playingStateStyle.alignment = TextAnchor.UpperCenter;

        

        playingStateStyle.normal.textColor = Corr(50, 50, 50, 255);
        if (GUI.Button(new Rect(0, 335, 285, 25), "@ REPLAY @", playingStateStyle))
        {
            loadSwitch = true;
            startTime = System.DateTime.Now.ToString("h:mm:ss.ff");
            timecalcu = true;

            ballXpos = Random.Range(10, 270);
            ballYpos = 100;
            currentScore = 0;
            scrollBgSpeed = 0.3f;
            stage = 1;
            xdir = 0.7f;
            ydir = 0.7f;
            nextStageNum = 0;
            StageSetting(2, 4);
            STATE = state.PLAYING;
        }

        if(reptime <= 255 && reptime > 0) reptime -= repSpeed;
        if(reptime <= 0) reptime = 255;

        playingStateStyle.normal.textColor = Corr(255, 255, 255, (byte)reptime);
        GUI.Label(new Rect(0, 335, 285, 25), "@ REPLAY @", playingStateStyle);


    }


    void Footer()
    {
        GUI.Box(new Rect(0, 393, Screen.width+5, 15), "", footerStyle);
    }



    void BrickCreate() //브릭 1,0 입력
    {
        for (int i = 0; i < brickRow; i++)
        {
            for (int j = 0; j < brickCol; j++)
            {
                stageBricks[i, j] = 1;
            }
        }
    }


    void DrawBricksShadwo(int row, int col) // 브릭 쉐도우
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {

                if (stageBricks[i, j] == 1)
                {
                    pos.x = (scale.x * j);

                    if (i % 2 == 0)
                    {
                        if (j % 2 == 0) brickStyle.normal.background = DB.DBTexture[92];
                        else brickStyle.normal.background = DB.DBTexture[97];
                    }
                    else {
                        if (j % 2 == 0) brickStyle.normal.background = DB.DBTexture[97];
                        else brickStyle.normal.background = DB.DBTexture[92];
                    }

                    //shadow
                    GUI.color = Corr(50, 50, 50, 40);
                    GUI.Box(new Rect(pos.x, pos.y + 5, scale.x, scale.y), "", brickStyle);
                    GUI.color = Color.white;
                }

                else
                {
                    
                    pos.x = Screen.width + 100;
                }
                if ((ballXpos + 12 > pos.x && ballXpos  < pos.x + scale.x) && (ballYpos > pos.y && ballYpos < pos.y + scale.y))
                {
                    xdir *= -1;
                    ydir *= -1;
                    stageBricks[i, j] = 0;

                   

                    currentScore++;
                    nextStageNum++;
                    // next stage;
                    if(nextStageNum >= stageBricks.Length)
                    {

                        stage++;
                        nextStageNum = 0;
                        loadSwitch = true;
                        xdir = xdir + 0.05f;
                        ydir = ydir + 0.05;
                        ballXpos = Random.Range(10, 270);
                        ballYpos = 100;
                        if(stage % 5 == 0)
                        {
                            StageSetting(4, 4 + (stage - 1));
                        }
                        else
                        {
                            StageSetting(2, 4 + (stage - 1));
                        }
                        
                    }

                   // Debug.Log(currentScore);
                }
            }
            pos.y = (scale.y * i);

        }
    }


    void DrawBricks(int row, int col) // 브릭 그리기
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {

                if (stageBricks[i, j] == 1)
                {
                    pos.x = (scale.x * j);


                    if ((i + 1) % 2 == 0)
                    {
                        if (j % 2 == 0) brickStyle.normal.background = DB.DBTexture[97];
                        else brickStyle.normal.background = DB.DBTexture[92];
                    }
                    else
                    {
                        if (j % 2 == 0) brickStyle.normal.background = DB.DBTexture[92];
                        else brickStyle.normal.background = DB.DBTexture[97];
                    }

                    GUI.Box(new Rect(pos.x, pos.y, scale.x, scale.y), "", brickStyle);
                }

                else
                {

                    pos.x = Screen.width + 100;
                }
                if ((ballXpos+12 > pos.x && ballXpos < pos.x + scale.x) && (ballYpos > pos.y && ballYpos < pos.y + scale.y))
                {
                    xdir *= -1;
                    ydir *= -1;
                    stageBricks[i, j] = 0;



                    currentScore++;
                    nextStageNum++;
                    // next stage;
                    if (nextStageNum >= stageBricks.Length)
                    {

                        stage++;
                        nextStageNum = 0;
                        loadSwitch = true;
                        xdir = xdir + 0.05f;
                        ydir = ydir + 0.05;
                        ballXpos = Random.Range(10, 270);
                        ballYpos = 100;
                        if (stage % 5 == 0)
                        {
                            StageSetting(4, 4 + (stage - 1));
                        }
                        else
                        {
                            StageSetting(2, 4 + (stage - 1));
                        }

                    }

                    // Debug.Log(currentScore);
                }
            }
            pos.y = (scale.y * i);

        }
    }

    




    double speedUP = 0.1f;
    double speedUPT = 0f;

   
    void Balls()
    {
        if (speedUPT <= 2.0f && speedUPT > 0) speedUPT -= speedUP;

        ballXpos += (xdir+ speedUPT);
        ballYpos += (ydir+ speedUPT);
        if (ballXpos > Screen.width - 10) xdir *= -1;
        if (ballXpos < 0) xdir *= -1;

        if (ballYpos < 0) ydir *= -1;

        if (ballYpos > 308 + plusYpos && (ballXpos > padX-10  && ballXpos < padX + 21))
        {
            ydir *= -1;
            xdir = xdirValue;
            if (xdir > 0)
            {
                
                xdir *= -1;
                //xdir = xdirValue;
            }
            paddleEffect = 255;
        }
        if (ballYpos > 308 + plusYpos && (ballXpos > padX+21 && ballXpos < padX + 21*2))
        {
            ydir *= -1;
            xdir *= 0.1f;
            paddleEffect = 255;
        }
        if (ballYpos > 308 + plusYpos && (ballXpos > padX + (21*2) && ballXpos < padX + (21 * 3)))
        {
            ydir *= -1;
            xdir = xdirValue;
            if (xdir < 0)
            {
                
                xdir *= -1;
              //  xdir = -xdirValue;
            }
            paddleEffect = 255;
        }


        //if (ballYpos > 308 + plusYpos && (ballXpos > PaddlePosX + (paddleWidth / 3) && ballXpos < PaddlePosX + paddleWidth))
        //{
        //    ydir *= -1;
        //    if (xdir < 0) xdir *= -1;
        //    paddleEffect = 255;
        //}
        if (ballYpos > 350)
        {
            ballYpos = 100;
            timecalcu = false;
            scrollBgSpeed = 1;
            currentStage = 0;
            STATE = state.END;
        }
        
        //shadow
        GUI.color = Corr(50, 50, 50, 40);
        GUI.Box(new Rect((float)ballXpos, (float)ballYpos +5, 12, 12), "", ballStyle);
        GUI.color = Color.white;
        GUI.Box(new Rect((float)ballXpos, (float)ballYpos, 12, 12), "",ballStyle);
    }

    void NewBalls()
    {

        newballXpos += xdir;
        newballYpos += ydir;
        if (newballXpos > Screen.width - 10) xdir *= -1;
        if (newballXpos < 0) xdir *= -1;

        if (newballYpos < 0) ydir *= -1;

        if (newballYpos > 308 + plusYpos && (newballXpos > PaddlePosX - (paddleWidth / 3) && newballXpos < PaddlePosX + (paddleWidth / 3)))
        {
            ydir *= -1;
            if (xdir > 0) xdir *= -1;
            paddleEffect = 255;
        }
        if (newballYpos > 308 + plusYpos && (newballXpos > PaddlePosX + (paddleWidth / 3) && newballXpos < PaddlePosX + paddleWidth))
        {
            ydir *= -1;
            if (xdir < 0) xdir *= -1;
            paddleEffect = 255;
        }
        if (newballYpos > 350)
        {
            newballYpos = 100;
            timecalcu = false;
            scrollBgSpeed = 1;
            currentStage = 0;
            STATE = state.END;
        }

        //shadow
        GUI.color = Corr(50, 50, 50, 40);
        GUI.Box(new Rect((float)newballXpos, (float)newballYpos + 5, 12, 12), "", ballStyle);
        GUI.color = Color.white;
        GUI.Box(new Rect((float)newballXpos, (float)newballYpos, 12, 12), "", ballStyle);
    }




    // Screen.height - (89+9) 314
    // Screen.height - 89 323
    //  Screen.height - 85 327

   

    void Paddle()
    {

        padX = PaddlePosX + 18 - (paddleWidth / 2);

        if (paddleEffect > 0 )
        {
            paddleEffect -= paddleEffectSpeed;
        }

        
        // shadow
        GUI.color = Corr(50, 50, 50, 40);
        GUI.Box(new Rect(padX, 318 + 4 + plusYpos, paddleWidth, 7), "", paddleStyle);
        GUI.color = Color.white;

        //paddle
        GUI.Box(new Rect(padX, 318+ plusYpos, paddleWidth, 7), "",paddleStyle);

        //paddle effect
        GUI.color = Corr(255, 255, 255, (byte)paddleEffect);
        GUI.Box(new Rect(padX, 318 + plusYpos, paddleWidth, 7), "", paddleLigheStyle);
        GUI.color = Color.white;

        
       

        PaddlePosX = GUI.HorizontalSlider(new Rect(20, 327+ plusYpos, 244, 20), PaddlePosX, 0, 244);
        PaddleCont();


    }
    void PaddleCont()
    {
        Vector2 mousePosition = Event.current.mousePosition;
        if (mousePosition.x < 0) mousePosition.x = 0;
        if (mousePosition.x > 244) mousePosition.x = 244;
        PaddlePosX = mousePosition.x;

        //Debug.Log(mousePosition);

    }


    //컬러 간단
    Color32 Corr(byte r, byte g, byte b, byte a)
    {
        return new Color32(r, g, b, a);
    }

}
