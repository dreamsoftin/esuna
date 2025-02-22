﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class DEFINE_APP {//ApplictionDefine

    public static float ANSWERING_LIMIT_TIME = 4f;

    public enum STATUS_ENUM : int
    {
        PREPARE,
        START,
        TUTORIAL,
        COUNT,
        PLAY,
        FINISH,
        SHOW_RESLUT
    }

    public static class UGUI {
        public enum ANCHOR {
            UpperLeft = 0,
            UpperCenter = 1,
            UpperRight = 2,
            MiddleLeft = 3,
            MiddleCenter = 4,
            MiddleRight = 5,
            LowerLeft = 6,
            LowerCenter = 7,
            LowerRight = 8
        }

        public static Dictionary<ANCHOR, Vector2> ANCHOR_VECTOR2 = new Dictionary<ANCHOR, Vector2>(){
            {ANCHOR.UpperLeft,      new Vector2(0.0f,1.0f)},
            {ANCHOR.UpperCenter,    new Vector2(0.5f,1.0f)},
            {ANCHOR.UpperRight,     new Vector2(1.0f,1.0f)},

            {ANCHOR.MiddleLeft,     new Vector2(0.0f,0.5f)},
            {ANCHOR.MiddleCenter,   new Vector2(0.5f,0.5f)},
            {ANCHOR.MiddleRight,    new Vector2(1.0f,0.5f)},

            {ANCHOR.LowerLeft,      new Vector2(0.0f,0.0f)},
            {ANCHOR.LowerCenter,    new Vector2(0.5f,0.0f)},
            {ANCHOR.LowerRight,     new Vector2(1.0f,0.0f)},

        };

    }

    public enum ANSWER_TYPE_ENUM
    {
        TUTORIAL,
        PLAY,
        RESULT
    }


    public static Vector3 ANSWERING_COLLIDER_SIZE = new Vector3(0.20f, 0.20f, 0.20f);
    public static Vector3 NORMAL_COLLIDER_SIZE = new Vector3(0.07f, 0.10f, 0.18f);

    public static Vector3 ANSWERING_GRIP = new Vector3(-0.06f, -0.03f, 0f);
    public static Vector3 NORMAL_GRIP_R = new Vector3(-0.02f, 0f, 0f);

    public static Vector3 LEFT_HAND_MEASURE_POS_7 = new Vector3(0f, -0.05f, 0.1f);
    public static Vector3 LEFT_HAND_MEASURE_ROT_7 = new Vector3(0f, 0f, 90f);
    public static Vector3 RIGHT_HAND_MEASURE_POS_7 = new Vector3(0f, -0.05f, 0.1f);
    public static Vector3 RIGHT_HAND_MEASURE_ROT_7 = new Vector3(0f,0f, -90f);

    public static Vector3 LEFT_HAND_MEASURE_POS_4 = new Vector3(0f, 0.05f, 0.1f);
    public static Vector3 LEFT_HAND_MEASURE_ROT_4 = new Vector3(0f, 0f, -40f);
    public static Vector3 RIGHT_HAND_MEASURE_POS_4 = new Vector3(0f, 0.05f, 0.1f);
    public static Vector3 RIGHT_HAND_MEASURE_ROT_4 = new Vector3(-10f, 0f, 40f);

    public static Vector3 HAND_MEASURE_POS_NOR = new Vector3(0f, 0f, 0.1f);
    public static Vector3 HAND_MEASURE_ROT_NOR = new Vector3(0f, 0f, 0f);

	public static Vector3 DEFAULT_HAND_POS_R = new Vector3(0.1f, 1.1f, 0.5f);
	public static Vector3 DEFAULT_HAND_POS_L = new Vector3(-0.1f, 1.1f, 0.5f);

	/// <summary>
	/// R=右手、L=左手、C=両手
	/// </summary>
	public static OVRInput.Controller[] HAND_TARGET = new OVRInput.Controller[] { OVRInput.Controller.RTouch, OVRInput.Controller.LTouch, OVRInput.Controller.RTouch, OVRInput.Controller.Touch, OVRInput.Controller.LTouch, OVRInput.Controller.RTouch, OVRInput.Controller.Touch, OVRInput.Controller.LTouch };
    /*
    public static Dictionary<OVRInput.Controller, Vector3> SHOULDER_POS_DIC = new Dictionary<OVRInput.Controller, Vector3>()
    {
        { OVRInput.Controller.RTouch, BODY_SCALE.SHOULDER_POS_R},
        { OVRInput.Controller.LTouch, BODY_SCALE.SHOULDER_POS_L},
        { OVRInput.Controller.Touch, BODY_SCALE.SHOULDER_POS_C},
    };
    public static Dictionary<OVRInput.Controller, Vector3> HAND_POS_DIC = new Dictionary<OVRInput.Controller, Vector3>()
    {
        { OVRInput.Controller.RTouch, BODY_SCALE.HAND_LOCALPOS_R},
        { OVRInput.Controller.LTouch, BODY_SCALE.HAND_LOCALPOS_L},
        { OVRInput.Controller.Touch, BODY_SCALE.HAND_LOCALPOS_C},
    };
    */

    public static class BODY_SCALE
    {
		/*
        // WorldPosition ※フロントのみ
        public static Vector3 PLAYER_BASE_POS;
        public static Vector3 PLAYER_BASE_ROT;
        // LocalPosition ※BasePositionから
        // DEFINEで設定 ※フロントのみ ※BasePositionから
        public static Vector3 BACK_POS = new Vector3(0f, 0.5f, 0f);
        // CenterEyeの位置　※サーバー通信 ※BasePositionから
        public static Vector3 HEAD_POS;
        // HEADとHandの位置から決定。BACK_POSから　※フロントのみ
        public static Vector3 SHOULDER_POS_R
        {
            get
            {
                return new Vector3(HAND_POS_R.x, HAND_POS_R.y, HEAD_POS.z) - BACK_POS;
            }
        }
        // HEADとHandの位置から決定。BACK_POSから　※フロントのみ
        public static Vector3 SHOULDER_POS_L
        {
            get
            {
                return new Vector3(HAND_POS_L.x, HAND_POS_L.y, HEAD_POS.z) - BACK_POS;
            }
        }
        // HEADとHandの位置から決定。BACK_POSから　※フロントのみ
        public static Vector3 SHOULDER_POS_C
        {
            get
            {
                return new Vector3((HAND_POS_L.x + HAND_POS_R.x) / 2f, (HAND_POS_L.y + HAND_POS_R.y) / 2f, HEAD_POS.z) - BACK_POS;
            }
        }

        // 各コントローラーの位置　BasePositionから　※サーバー通信
        public static Vector3 HAND_POS_R = new Vector3(0.1f, 1.1f, 0.5f);
        public static Vector3 HAND_POS_L = new Vector3(-0.1f, 1.1f, 0.5f);
        public static Vector3 HAND_POS_C
        {
            get
            {
                return (HAND_POS_L + HAND_POS_R) / 2f;
            }
        }

        // 各コントローラーの位置　ShouldePositionから　※サーバーしない
        public static Vector3 HAND_LOCALPOS_R
        {
            get
            {
                return HAND_POS_R - SHOULDER_POS_R - BACK_POS;
            }
        }
        public static Vector3 HAND_LOCALPOS_L
        {
            get
            {
                return HAND_POS_L - SHOULDER_POS_L - BACK_POS;
            }
        }
        public static Vector3 HAND_LOCALPOS_C
        {
            get
            {
                return (HAND_LOCALPOS_L + HAND_LOCALPOS_R) / 2f;
            }
        }
		*/


        public static int[] DIAGNOSIS_DIRECTS = new int[]{1,2,4,7};

        public static int DIAGNOSIS_COUNT = 10;

        public static Dictionary<int, int> DIAGNOSIS_COUNT_DIC = new Dictionary<int, int>()
        {
            {1, 14},
            {2, 14},
            {3, 12},
            {4, 12},
            {5, 12},
            {6, 9},
            {7, 9},
            {8, 9}
        };

		/*
        public static void SetDefine()
        {
			_GOAL_DIC = new Dictionary<int, Dictionary<string, Vector3>>()
            {
                    {1, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, -55f, 0f) }, { SHOULDER_ROT, new Vector3(0f, -55f, 0f) } } },
                    {2, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, 55f, 0f) }, { SHOULDER_ROT, new Vector3(0f, 55f, 0f) } } },
                    {4, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(-20f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(-80f, 0f, 0f) } } },
                    {7, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(80f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(30f, 0f, 0f) } } }
            };

            _DIAGNOSIS_ROT_MAX = new Dictionary<int, Dictionary<string, Vector3>>
            {
                {1, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, -55f, 0f) }, { SHOULDER_ROT, new Vector3(0f, -55f, 0f) } } },
                {2, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, 55f, 0f) }, { SHOULDER_ROT, new Vector3(0f, 55f, 0f) } } },
                {4, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(-20f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(-80f, 0f, 0f) } } },
                {7, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(80f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(30f, 0f, 0f) } } }
            };

            _GOAL_CURRENT_DIC = new Dictionary<int, Dictionary<string, Vector3>>
            {
                {1, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(0f, 0f, 0f) } } },
                {2, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(0f, 0f, 0f) } } },
                {3, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(0f, 0f, 0f) } } },
                {4, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(0f, 0f, 0f) } } },
                {5, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(0f, 0f, 0f) } } },
                {6, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(0f, 0f, 0f) } } },
                {7, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(0f, 0f, 0f) } } },
                {8, new Dictionary<string, Vector3>{ { BACK_ROT, new Vector3(0f, 0f, 0f) }, { SHOULDER_ROT, new Vector3(0f, 0f, 0f) } } }
            };

            _GOAL_DIC.Add(3, new Dictionary<string, Vector3> { { BACK_ROT, new Vector3(0f, GOAL_DIC[1][BACK_ROT].y, 0f) }, { SHOULDER_ROT, new Vector3(GOAL_DIC[4][SHOULDER_ROT].x / 2f, GOAL_DIC[1][SHOULDER_ROT].y, 0f) } });
            _GOAL_DIC.Add(5, new Dictionary<string, Vector3> { { BACK_ROT, new Vector3(0f, GOAL_DIC[2][BACK_ROT].y, 0f) }, { SHOULDER_ROT, new Vector3(GOAL_DIC[4][SHOULDER_ROT].x / 2f, GOAL_DIC[2][SHOULDER_ROT].y, 0f) } });
            _GOAL_DIC.Add(6, new Dictionary<string, Vector3> { { BACK_ROT, new Vector3(GOAL_DIC[7][BACK_ROT].x, GOAL_DIC[1][BACK_ROT].y, 0f) }, { SHOULDER_ROT, new Vector3(GOAL_DIC[7][SHOULDER_ROT].x, 0f, 0f) } });
            _GOAL_DIC.Add(8, new Dictionary<string, Vector3> { { BACK_ROT, new Vector3(GOAL_DIC[7][BACK_ROT].x, GOAL_DIC[2][BACK_ROT].y, 0f) }, { SHOULDER_ROT, new Vector3(GOAL_DIC[7][SHOULDER_ROT].x, 0f, 0f) } });

            DIAGNOSIS_ROT_MAX.Add(3, new Dictionary<string, Vector3> { { BACK_ROT, new Vector3(0f, GOAL_DIC[1][BACK_ROT].y, 0f) }, { SHOULDER_ROT, new Vector3(GOAL_DIC[4][SHOULDER_ROT].x / 2f, GOAL_DIC[1][SHOULDER_ROT].y, 0f) } });
            DIAGNOSIS_ROT_MAX.Add(5, new Dictionary<string, Vector3> { { BACK_ROT, new Vector3(0f, GOAL_DIC[2][BACK_ROT].y, 0f) }, { SHOULDER_ROT, new Vector3(GOAL_DIC[4][SHOULDER_ROT].x / 2f, GOAL_DIC[2][SHOULDER_ROT].y, 0f) } });
            DIAGNOSIS_ROT_MAX.Add(6, new Dictionary<string, Vector3> { { BACK_ROT, new Vector3(GOAL_DIC[7][BACK_ROT].x, GOAL_DIC[1][BACK_ROT].y, 0f) }, { SHOULDER_ROT, new Vector3(GOAL_DIC[7][SHOULDER_ROT].x, 0f, 0f) } });
            DIAGNOSIS_ROT_MAX.Add(8, new Dictionary<string, Vector3> { { BACK_ROT, new Vector3(GOAL_DIC[7][BACK_ROT].x, GOAL_DIC[2][BACK_ROT].y, 0f) }, { SHOULDER_ROT, new Vector3(GOAL_DIC[7][SHOULDER_ROT].x, 0f, 0f) } });

			SetDefineCurrentDiagonal();
        }


        public static void SetDefineDiagonal()
        {
			_GOAL_DIC[3][BACK_ROT] = new Vector3(0f, GOAL_DIC[1][BACK_ROT].y, 0f);
            _GOAL_DIC[3][SHOULDER_ROT] = new Vector3(GOAL_DIC[4][SHOULDER_ROT].x / 2f, GOAL_DIC[1][SHOULDER_ROT].y, 0f);
            _GOAL_DIC[5][BACK_ROT] = new Vector3(0f, GOAL_DIC[2][BACK_ROT].y, 0f);
            _GOAL_DIC[5][SHOULDER_ROT] = new Vector3(GOAL_DIC[4][SHOULDER_ROT].x / 2f, GOAL_DIC[2][SHOULDER_ROT].y, 0f);
            _GOAL_DIC[6][BACK_ROT] = new Vector3(GOAL_DIC[7][BACK_ROT].x, GOAL_DIC[1][BACK_ROT].y, 0f);
            _GOAL_DIC[6][SHOULDER_ROT] = new Vector3(GOAL_DIC[7][SHOULDER_ROT].x, 0f, 0f);
            _GOAL_DIC[8][BACK_ROT] = new Vector3(GOAL_DIC[7][BACK_ROT].x, GOAL_DIC[2][BACK_ROT].y, 0f);
            _GOAL_DIC[8][SHOULDER_ROT] = new Vector3(GOAL_DIC[7][SHOULDER_ROT].x, 0f, 0f);

            SetDefineCurrentDiagonal();

        }


        public static void SetDefineCurrentDiagonal()
        {
			for (int i = 1; i <= _GOAL_DIC.Count; i++)
            {
                Vector3 backRot = DEFINE_APP.BODY_SCALE.GOAL_DIC[i][DEFINE_APP.BODY_SCALE.BACK_ROT] - (DEFINE_APP.BODY_SCALE.GOAL_DIC[i][DEFINE_APP.BODY_SCALE.BACK_ROT] * (1f / 3f));
                Vector3 shoulderRot = DEFINE_APP.BODY_SCALE.GOAL_DIC[i][DEFINE_APP.BODY_SCALE.SHOULDER_ROT] - (DEFINE_APP.BODY_SCALE.GOAL_DIC[i][DEFINE_APP.BODY_SCALE.SHOULDER_ROT] * (1f / 3f));

                Vector3 backRotMin = DEFINE_APP.BODY_SCALE.DIAGNOSIS_ROT_MAX[i][DEFINE_APP.BODY_SCALE.BACK_ROT] / (float)DEFINE_APP.BODY_SCALE.DIAGNOSIS_COUNT_DIC[i];
                Vector3 shoulderRotMin = DEFINE_APP.BODY_SCALE.DIAGNOSIS_ROT_MAX[i][DEFINE_APP.BODY_SCALE.SHOULDER_ROT] / (float)DEFINE_APP.BODY_SCALE.DIAGNOSIS_COUNT_DIC[i];


                float resultXBack = (Mathf.Abs(backRot.x) <= Mathf.Abs(backRotMin.x)) ? backRotMin.x : backRot.x;

                float resultXShoulder = (Mathf.Abs(shoulderRot.x) <= Mathf.Abs(shoulderRotMin.x)) ? shoulderRotMin.x : shoulderRot.x;

                float resultYBack = (Mathf.Abs(backRot.y) <= Mathf.Abs(backRotMin.y)) ? backRotMin.y : backRot.y;

                float resultYShoulder = (Mathf.Abs(shoulderRot.y) <= Mathf.Abs(shoulderRotMin.y)) ? shoulderRotMin.y : shoulderRot.y;

                float resultZBack = (Mathf.Abs(backRot.z) <= Mathf.Abs(backRotMin.z)) ? backRotMin.z : backRot.z;

                float resultZShoulder = (Mathf.Abs(shoulderRot.z) <= Mathf.Abs(shoulderRotMin.z)) ? shoulderRotMin.z : shoulderRot.z;

                DEFINE_APP.BODY_SCALE._GOAL_CURRENT_DIC[i][DEFINE_APP.BODY_SCALE.BACK_ROT] = new Vector3(resultXBack, resultYBack, resultZBack);
                DEFINE_APP.BODY_SCALE._GOAL_CURRENT_DIC[i][DEFINE_APP.BODY_SCALE.SHOULDER_ROT] = new Vector3(resultXShoulder, resultYShoulder, resultZShoulder);
            }
        }


		/// <summary>
		/// 8方向の最大角度を保存。
		/// </summary>
        private static Dictionary<int, Dictionary<string, Vector3>> _GOAL_DIC;
        public static Dictionary<int, Dictionary<string, Vector3>> GOAL_DIC {
            get
            {
                if (_GOAL_DIC == null){
                    SetDefine();
                }
                return _GOAL_DIC;
            }
        }
        
		private static Dictionary<int, Dictionary<string, Vector3>> _GOAL_CURRENT_DIC;
		public static Dictionary<int, Dictionary<string, Vector3>> GOAL_CURRENT_DIC
		{
			get
			{
				if (_GOAL_CURRENT_DIC == null)
				{
					SetDefine();
				}
				return _GOAL_CURRENT_DIC;
			}
		}

		private static Dictionary<int, Dictionary<string, Vector3>> _DIAGNOSIS_ROT_MAX;
		public static Dictionary<int, Dictionary<string, Vector3>> DIAGNOSIS_ROT_MAX
		{
			get
			{
				if (_DIAGNOSIS_ROT_MAX == null)
				{
					SetDefine();
				}
				return _DIAGNOSIS_ROT_MAX;
			}
		}
        */
		public static string BACK_ROT = "BACK_ROT";
        public static string SHOULDER_ROT = "SHOULDER_ROT";

		/// <summary>
		/// 体を回旋させる時の回転軸
		/// </summary>
		public static readonly Dictionary<int, Dictionary<string, Vector3>> ROT_AXIS = new Dictionary<int, Dictionary<string, Vector3>> {

			{ 1, new Dictionary<string, Vector3> { { BACK_ROT, Vector3.down }, { SHOULDER_ROT, Vector3.down } } },
			{ 2, new Dictionary<string, Vector3> { { BACK_ROT, Vector3.up }, { SHOULDER_ROT, Vector3.up } } },
			{ 3, new Dictionary<string, Vector3> { { BACK_ROT, Vector3.down }, { SHOULDER_ROT, new Vector3(-0.550f, -0.785f, 0f/*-0.286f*/) } } },
			{ 4, new Dictionary<string, Vector3> { { BACK_ROT, Vector3.left }, { SHOULDER_ROT, Vector3.left } } },
			{ 5, new Dictionary<string, Vector3> { { BACK_ROT, Vector3.up }, { SHOULDER_ROT, new Vector3(-0.550f,  0.785f,  0f/*0.286f*/) } } },
			{ 6, new Dictionary<string, Vector3> { { BACK_ROT, new Vector3(0.777f, -0.482f,  0f/*0.405f*/) }, { SHOULDER_ROT, Vector3.right } } },
			{ 7, new Dictionary<string, Vector3> { { BACK_ROT, Vector3.right }, { SHOULDER_ROT, Vector3.right } } },
			{ 8, new Dictionary<string, Vector3> { { BACK_ROT, new Vector3(0.777f,  0.482f, 0f/*-0.405f*/) }, { SHOULDER_ROT, Vector3.right } } }
		};


		/// <summary>
		/// 体を回旋させる時、腰と肩それぞれを回転軸に対して何度まで回転できるかの定義
		/// </summary>
		public static readonly Dictionary<int, Dictionary<string, float>> DIAGNOSIS_ROT_MAX = new Dictionary<int, Dictionary<string, float>> {

			{ 1, new Dictionary<string, float> { { BACK_ROT, 85f }, { SHOULDER_ROT, 55f } } },
			{ 2, new Dictionary<string, float> { { BACK_ROT, 85f }, { SHOULDER_ROT, 55f } } },
			{ 3, new Dictionary<string, float> { { BACK_ROT, 85f }, { SHOULDER_ROT, 67f } } },
			{ 4, new Dictionary<string, float> { { BACK_ROT, 20f }, { SHOULDER_ROT, 100f } } },
			{ 5, new Dictionary<string, float> { { BACK_ROT, 85f }, { SHOULDER_ROT, 67f } } },
			{ 6, new Dictionary<string, float> { { BACK_ROT, 94f }, { SHOULDER_ROT, 30f } } },
			{ 7, new Dictionary<string, float> { { BACK_ROT, 80f }, { SHOULDER_ROT, 30f } } },
			{ 8, new Dictionary<string, float> { { BACK_ROT, 94f }, { SHOULDER_ROT, 30f } } }
		};

		private static Dictionary<int, Dictionary<string, float>> _DIAGNOSIS_ROT_MAX_RATIO = null;
		/// <summary>
		/// 体を最大まで回旋させた時、腰と肩それぞれがしめる回転の割合
		/// </summary>
		/// <value>The diagnosis rot max ratio.</value>
		public static Dictionary<int, Dictionary<string, float>> DIAGNOSIS_ROT_MAX_RATIO {
			get {
				if (_DIAGNOSIS_ROT_MAX_RATIO == null) {
					_DIAGNOSIS_ROT_MAX_RATIO = new Dictionary<int, Dictionary<string, float>>();

					foreach (int direction in DIAGNOSIS_ROT_MAX.Keys) {
						_DIAGNOSIS_ROT_MAX_RATIO.Add(direction, new Dictionary<string, float>());

						float sum = 0f;
						foreach (KeyValuePair<string, float> kvp in DIAGNOSIS_ROT_MAX[direction]) {
							sum += kvp.Value;
						}
						foreach (KeyValuePair<string, float> kvp in DIAGNOSIS_ROT_MAX[direction]) {
							_DIAGNOSIS_ROT_MAX_RATIO[direction].Add(kvp.Key, kvp.Value / sum);
						}
					}
				}
				return _DIAGNOSIS_ROT_MAX_RATIO;
			}
		}


		public static Dictionary<int, float> SHOULDER_ROT_Z = new Dictionary<int, float>()
        {
            {1,-90f},
            {2,90f},
            {3,-135f},
            {4,180f},
            {5,135f},
            {6,-45f},
            {7,0f},
            {8,45f}
        };


        public static Dictionary<int, Vector3> NEST_ROT = new Dictionary<int, Vector3>()
        {
            {1, new Vector3(0f, 0f, -90f)},
            {2,new Vector3(0f, 0f, 90f)},
            {3,new Vector3(0f, 0f, -135f)},
            {4,new Vector3(0f, 0f, 180f)},
            {5,new Vector3(0f, 0f, 135f)},
            {6,new Vector3(-60f, 20f, 0f)},
            {7,new Vector3(-60f, 0f, 0f)},
            {8,new Vector3(-60f, -20f, 0f)}
        };


        //public static Dictionary<int, int> ARM_ROT_SIGN = new Dictionary<int, int>()
        //{
        //    {1,-1},
        //    {2,1},
        //    {3,-1},
        //    {4,-1},
        //    {5,-1},
        //    {6,1},
        //    {7,1},
        //    {8,1}
        //};
    }



    /// <summary>
    /// 測定開始してから次のボールに触れないために次に移動する時間。
    /// </summary>
    public static float DIAGNOSIS_WAIT_TIME = 3f;

	/*
    public static Dictionary<int, int> NRS_PRE = new Dictionary<int, int>()
    {
        {1, -1 },
        {2, -1 },
        {3, -1 }
    };

    public static Dictionary<int, int> NRS_POST = new Dictionary<int, int>()
    {
        {1, -1 },
        {2, -1 },
        {3, -1 }
    };
    */
}


