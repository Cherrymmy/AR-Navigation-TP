//using Firebase.Auth;
//using Firebase.Extensions;
using TMPro;
using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{
    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    public GameObject main;

    //private FirebaseAuth auth;
    public TextMeshProUGUI userId;

    private void Start()
    {
        //auth = FirebaseAuth.DefaultInstance;
    }
    /// <summary>
    /// 버튼 상호작용용 함수 
    /// </summary>
    public void OnLoginButtonClicked()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        //LoginToFirebase(email, password);
    }


    /// <summary>
    /// 로그인 시도
    /// </summary>
    /// <param name="email">아이디</param>
    /// <param name="password">비밀번호</param>
    /*
    private void LoginToFirebase(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Login Failed.");
                // 로그인 실패 처리
                Transform child = transform.Find("ID_Text");
                child.gameObject.SetActive(true);
            }
            else
            {
                // Firebase 로그인 성공
                FirebaseUser user = task.Result.User;
                Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.UserId);
                Debug.Log("로그인 성공");

                // 여기서 사용자 ID를 검증하고, 검증 성공 시 Photon에 연결합니다.
                ConnectToPhoton(user.UserId);
                userId.text = user.UserId;
                this.gameObject.SetActive(false);
                main.SetActive(true);
            }
        });
    }
    */
}