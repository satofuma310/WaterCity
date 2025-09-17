
#if UNITY_EDITOR

using UnityEditor;

/// <summary>
/// エディタ再生前にアセットを更新します。
/// </summary>
public class RefreshPlay
{
    // 説明:
    // スクリプトを変更してUnityに戻ってくるたびにリロードが走ると
    // 固まったような状態になって度々作業が中断されてしまう。
    // 
    // これを回避するためにまず、以下の設定をOFFに設定する
    // Edit > Prefe.. > Asset Pipiline > Auto Refresh
    //
    // そうすると [Ctrl + R] でしか更新されなくなるが
    // 今度は Play したときに更新されずに実行されてしまうので
    // 以下のスクリプトで実行前にアセットの強制リフレッシュを行う動作を割り込ませて
    // 実行時には最新の状態が保たれるようにするスクリプト

    [InitializeOnLoadMethod]
    public static void Run()
    {
        EditorApplication.playModeStateChanged += (PlayModeStateChange state) =>
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                AssetDatabase.Refresh();
            }
        };
    }
}

#endif
