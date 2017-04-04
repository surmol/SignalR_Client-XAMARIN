using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.AspNet.SignalR.Client;
using Android.Graphics;

namespace SignalR_Client
{
    [Activity(Label = "SignalR_Client", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public string UserName;
        public int BackgroundColor;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            start();
            //GetInfo getInfo = new GetInfo();
            //getInfo.OnGetInfoComplete += GetInfo_OnGetInfoComplete;
            //getInfo.Show(FragmentManager, "GetInfo");
        }
        public enum DangerLevel
        {
            Undefined = -1,
            Normal = 0,
            Acceptable = 1,
            Suspicious = 2,
            Warning = 3,
            Danger = 4,
            Alarm = 5
        }
        public enum Pose
        {
            Multiple = -2,
            NotProcessed = -1,
            Unknown = 0,
            Moving = 1,
            Lying = 2,
            LyingArmToRight = 3,
            LyingArmToLeft = 4,
            SittingLegsOnBed = 5,
            SittingLegsDownToRight = 6,
            SittingLegsDownToLeft = 7,
            StandingToRight = 8,
            StandingToLeft = 9,
            SuspiciousToRight = 10,
            SuspiciousToLeft = 11
        }
        public static class Camera
        {
            public static long cameraId { get; set; }
            public static Pose pose { get; set; }
            public static DangerLevel dangerLevel { get; set; }
            public static DateTime timestamp { get; set; }

            static Camera()
            {

            }
        }


        private async void start()
        {
       

            var hubConnection = new HubConnection("http://192.168.1.63:8089/signalr");
            var chatHubProxy = hubConnection.CreateHubProxy("FramesHub");

            chatHubProxy.On<long, Pose, DangerLevel, DateTime>("broadcastPoseChanged", (cameraId, pose, dangerLevel, timestamp) =>
            {
                //UpdateChatMessage has been called from server
                
                RunOnUiThread(() =>
                {
                    TextView txt = new TextView(this);
                    txt.Text = "Camera id:" + cameraId+ " Pose:" + " Danger Level:"+ dangerLevel + " Time:" + timestamp;
                    txt.SetTextSize(Android.Util.ComplexUnitType.Sp, 20);
                    txt.SetPadding(10, 10, 10, 10);

                    switch (Convert.ToInt32(dangerLevel))
                    {
                        case 1:
                            txt.SetTextColor(Color.Red);
                            break;

                        case 2:
                            txt.SetTextColor(Color.DarkGreen);
                            break;

                        case 3:
                            txt.SetTextColor(Color.Blue);
                            break;

                        default:
                            txt.SetTextColor(Color.Black);
                            break;

                    }

                    txt.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                    {
                        TopMargin = 10,
                        BottomMargin = 10,
                        LeftMargin = 10,
                        RightMargin = 10,
                        Gravity = GravityFlags.Right
                    };

                    FindViewById<LinearLayout>(Resource.Id.llChatMessages)
                            .AddView(txt);
                });
            });
               
            try
            {
                await hubConnection.Start();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            //FindViewById<Button>(Resource.Id.btnSend).Click += async (o, e2) =>
            //{
            //    var message = FindViewById<EditText>(Resource.Id.txtChat).Text;

            //    await chatHubProxy.Invoke("SendMessage", new object[] { message, BackgroundColor, UserName });
            //};

        }
    }
}

