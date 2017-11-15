using UnityEngine.SceneManagement;

namespace GameFW.UI
{
    /// <summary>
    /// UI控件名字
    /// </summary>
    public class UIWidgetNames
    {
        #region login scene ui widget names
        public static string RegisterPanelName = "CanvasregisterPanel";
        public static string RegisterPanelRegisterButtonName = "CanvasregisterPanelImagebtn_register";
        public static string RegisterAccountInputfieldName = "CanvasregisterPanelImageinp_registerAccount";
        public static string RegisterPasswordInputfieldName = "CanvasregisterPanelImageinp_registerPwd";
        public static string RegisterPwdAgainInputfieldName = "CanvasregisterPanelImageinp_registerPwdAgain";
        public static string RegisterButtonName = "CanvasloginPanelLoginAreabtn_registerPanel"; 
        public static string RegisterQuitButtonName = "CanvasregisterPanelImagebtn_registerClose";

        public static string LoginAccountInputfieldName = "CanvasloginPanelLoginAreainp_loginAccount";
        public static string LoginPasswordInputfieldName = "CanvasloginPanelLoginAreainp_loginPwd";
        public static string LoginButtonName = "CanvasloginPanelLoginAreabtn_enter";
        #endregion

        #region match scene ui widget names
        public static string CreateUserPanelName = "MATCH_UIcreateUserPanel";
        public static string CreateUserPanelTextName = "MATCH_UIcreateUserPanelinp_name";
        public static string CreateUserEnsureBtnName = "MATCH_UIcreateUserPanelbtn_create";

        public static string InfoPanelNameTextName = "MATCH_UIinfoPanelPlayerInfoExpHUDtxt_playerName";
        public static string InfoPanelLevelTextName = "MATCH_UIinfoPanelPlayerInfotxt_level";
        public static string InfoPanelExpSliderName = "MATCH_UIinfoPanelPlayerInfoExpHUD";
        public static string MatchBtnName = "MATCH_UIinfoPanelbtn_match";
        public static string MatchTextHintName = "MATCH_UIinfoPanelmatchHintPaneltxt_matchHint";
        public static string MatchBtnTextName = "MATCH_UIinfoPanelbtn_matchtxt_matchBtn";

        #endregion

        #region select scene ui widget names
        public static string PlayerLeftListName = "SELECT_UILeftListLeftHeroList";
        public static string PlayerRightListName = "SELECT_UIRightListRightHeroList";
        public static string SelectEnsureButtonName = "SELECT_UISelectHerosPanelHeroSettingbtn_selectEnsure";
        public static string ReadyButtonName = "SELECT_UISelectHerosPanelHeroSettingbtn_ready";
        public static string ReadyButtonTextName = "SELECT_UISelectHerosPanelHeroSettingbtn_readyText";
        public static string HeroGridBoxName = "SELECT_UISelectHerosPanelHeroGirdBox";
        public static string ChatSendButtonName = "SELECT_UIChatlFramebtn_send";
        public static string ChatInputName = "SELECT_UIChatlFrameinp_chat";
        public static string DetailTextName = "SELECT_UIDetailFrameText";
        public static string TimerTextName = "SELECT_UITimerText";
        #endregion

        public static string ErrorPanelName {
            get {
                switch (SceneManager.GetActiveScene().name) {
                    case "Login":
                        return "CanvaserrorPanel";
                    case "Match":
                        return "MATCH_UIerrorPanel";
                    case "Select":
                        return "";
                    case "Fight":
                        return "";
                    default:
                        return null;
                }
            }
        }

        public static string ErrorEnsureButtonName
        {
            get
            {
                switch (SceneManager.GetActiveScene().name)
                {
                    case "Login":
                        return "CanvaserrorPanelshowBoxbtn_errorEnsure";
                    case "Match":
                        return "MATCH_UIerrorPanelshowBoxbtn_errorEnsure";
                    case "Select":
                        return "";
                    case "Fight":
                        return "";
                    default:
                        return null;
                }
            }
        }
        public static string ErrorTextName
        {
            get
            {
                switch (SceneManager.GetActiveScene().name)
                {
                    case "Login":
                        return "CanvaserrorPanelshowBoxtxt_errorInfo";
                    case "Match":
                        return "MATCH_UIerrorPanelshowBoxtxt_errorInfo";
                    case "Select":
                        return "";
                    case "Fight":
                        return "";
                    default:
                        return null;
                }
            }
        }

    }
}
