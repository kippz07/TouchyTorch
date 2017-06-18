using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Support.V7.App;
using Android.Hardware;
using Java.Lang;
using Android.Util;
using static Android.Hardware.Camera;
using Android.Content.PM;

namespace TouchyTorch
{
    [Activity(Label = "TouchyTorch", MainLauncher = true, Icon = "@drawable/lightbulbOn", Theme = "@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {

        private Camera camera;
        private Parameters mParams;
        private bool isFlashLight = false;
        ImageView imageView;
        string tag = "log-tag";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            imageView = FindViewById<ImageView>(Resource.Id.imageView);

            if (bundle != null)
            {
                isFlashLight = bundle.GetBoolean("light_status", false);
            }
            

            imageView.Click += delegate
            {
                FlashLight();
            };

            bool hasFlash = ApplicationContext.PackageManager.HasSystemFeature(Android.Content.PM.PackageManager.FeatureCameraFlash);
            if (!hasFlash)
            {
                Android.App.AlertDialog alert = new Android.App.AlertDialog.Builder(this).Create();
                alert.SetTitle("Error");
                alert.SetMessage("Your device does not support flash light");
                alert.SetButton("OK", (s, e) => { return; });
                alert.Show();
            }

            getCamera();
        }

        private void getCamera()
        {
            if (camera == null)
            {
                try
                {
                    camera = Camera.Open();
                    mParams = camera.GetParameters();
                }
                catch(RuntimeException ex)
                {
                    Log.Info("ERROR", ex.Message);
                }
            }
        }

        private void FlashLight()
        {
            if (camera == null || mParams == null)
            {
                return;
            }
            if (!isFlashLight)
            {               
                mParams = camera.GetParameters();
                mParams.FlashMode = Parameters.FlashModeTorch;
                camera.SetParameters(mParams);
                camera.StartPreview();
                isFlashLight = true;
                imageView.SetImageResource(Resource.Drawable.lightbulbOn);
                Log.Info(tag, "isFlashLight is now true");
            }
            else
            {               
                mParams = camera.GetParameters();
                mParams.FlashMode = Parameters.FlashModeOff;
                camera.SetParameters(mParams);
                camera.StartPreview();
                isFlashLight = false;
                imageView.SetImageResource(Resource.Drawable.lightbulbOff);
                Log.Info(tag, "isFlashLight is now false");
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean("light_status", isFlashLight);

            base.OnSaveInstanceState(outState);
        }

        public override void OnBackPressed()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            base.OnBackPressed();

        }

    }
}

