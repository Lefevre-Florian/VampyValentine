// Author : Renaudin Matteo
namespace Com.IsartDigital.Platformer.UI.Gameplay
{
    public class BtnDash : BtnHUD
    {
        protected override void OnBtn()
        {
            base.OnBtn();
            PlayerBis.PlayerController.GetInstance().isDashButtonPressed = true;
            PlayerBis.PlayerController.GetInstance().SetModeDash();
        }
    }
}
