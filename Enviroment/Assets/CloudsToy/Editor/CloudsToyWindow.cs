using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class CloudsToyWindow : EditorWindow
{
    private static int windowWidth = 500;
    private static int windowHeight = 700;
    private Texture2D myCloudsToyIcon;


    [MenuItem("Window/CloudsToy Welcome")]
    public static void OpenCloudsToyWindow()
    {
        CloudsToyWindow window = GetWindow<CloudsToyWindow>();
        
        window.titleContent = new GUIContent("CloudsToy");
        window.titleContent.image = EditorGUIUtility.IconContent("_Help").image;
        window.autoRepaintOnSceneChange = true;
        window.maxSize = new Vector2(windowWidth, windowHeight);
        window.minSize = new Vector2(windowWidth, windowHeight);
        window.Show();
        window.Focus();
    }

    private void OnEnable()
    {
        if (CloudsToy.isCloudsToyFree)
        {
            myCloudsToyIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/CloudsToy/Textures/Icons/CloudsToyIconFreev2.png", typeof(Texture2D));
        }
        else
        {
            myCloudsToyIcon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/CloudsToy/Textures/Icons/CloudsToyIconSrcv2.png", typeof(Texture2D));
        }
    }

    private void OnGUI()
    {
        DrawCloudsToyIcon();
        DrawPanel();
       
        if (GUI.changed) Repaint();
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }

    private void DrawPanel()
    {
        EditorGUILayout.BeginVertical();

        #region Welcome
        EditorGUILayout.Space();
        GUIContent contentCloud = new GUIContent("Welcome to CloudsToy.");
        GUI.changed = false;
        EditorGUILayout.LabelField(contentCloud, GUILayout.Width(windowWidth));
        DrawSpaces(2);
        contentCloud = new GUIContent("CloudsToy is a cloud generator tool that makes a very good job creating clouds" +
                                      "in the sky. It's simple to use and can be used in mobile.\n" +
                                      "You'll loose track of time by playing with it!");
        //EditorGUILayout.LabelField(contentCloud, GUILayout.Width(windowWidth), GUILayout.Height(60));
        EditorGUILayout.HelpBox("CloudsToy is a cloud generator tool that makes a very good job creating clouds\n" +
                                "in the sky. It's simple to use and can be used in mobile.\n" +
                                "You'll loose track of time by playing with it!", MessageType.Info);
        DrawSpaces(6);
        #endregion

        #region Demo scene

        contentCloud = new GUIContent("You can start by opening a demo scene and play with the CloudsToyMngr prefab.");
        EditorGUILayout.LabelField(contentCloud, GUILayout.Width(windowWidth));
        if (GUILayout.Button(new GUIContent(" Open Demo Scene", EditorGUIUtility.IconContent("UnityLogo").image), GUILayout.MaxHeight(20f), GUILayout.MaxWidth(185f)))
        {
            EditorSceneManager.OpenScene("Assets/CloudsToy/Scenes/1.-CloudsToy.unity");
        }
        #endregion

        #region Documentation
        EditorGUILayout.Space();
        contentCloud = new GUIContent("Also you can take a look to the documentacion included.");
        EditorGUILayout.LabelField(contentCloud, GUILayout.Width(windowWidth));
        if (GUILayout.Button(new GUIContent(" Open Documentation", EditorGUIUtility.IconContent("_Help").image), GUILayout.MaxWidth(185f)))
        {
            string URL = "file://" + Application.dataPath + "/CloudsToy/Documentation.pdf";
            Application.OpenURL(URL);
        }
        DrawSpaces(8);
        #endregion

        #region RateCloudsToy
        contentCloud = new GUIContent("You can rate CloudsToy (or Cloudstoy Source) in the Asset Store.");
        EditorGUILayout.LabelField(contentCloud, GUILayout.Width(windowWidth));

        Color myColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.green;
        EditorGUILayout.BeginHorizontal();

        if (CloudsToy.isCloudsToyFree)
        {
            if (GUILayout.Button(new GUIContent(" Rate CloudsToy", EditorGUIUtility.IconContent("sv_label_1").image), GUILayout.MaxHeight(20f), GUILayout.MaxWidth(145f)))
            {
                string URL = "https://assetstore.unity.com/packages/tools/particles-effects/cloudstoy-35559/";
                Application.OpenURL(URL);
            }
        }
        else
        {
            if (GUILayout.Button(new GUIContent(" Rate CloudsToy Source", EditorGUIUtility.IconContent("sv_label_1").image), GUILayout.MaxHeight(20f), GUILayout.MaxWidth(185f)))
            {
                string URL = "https://assetstore.unity.com/packages/tools/particles-effects/cloudstoy-source-99415";
                Application.OpenURL(URL);
            }
        }

        GUI.backgroundColor = myColor;
        EditorGUILayout.EndHorizontal();
        DrawSpaces(2);
#endregion

        #region MoreAssets
        contentCloud = new GUIContent("Here you have more assets from Jocyf.");
        EditorGUILayout.LabelField(contentCloud, GUILayout.Width(windowWidth));
        if (GUILayout.Button(new GUIContent(" More Assets", EditorGUIUtility.IconContent("WaitSpin01").image), GUILayout.MaxHeight(20f), GUILayout.MaxWidth(185f)))
        {
            string URL = "https://assetstore.unity.com/publishers/2861/";
            Application.OpenURL(URL);
        }
        DrawSpaces(12);
        #endregion

        #region Copyright
        EditorGUILayout.LabelField("Copyright © 2019 Jocyf");
        #endregion

        EditorGUILayout.EndVertical();
    }

    // Draw the CloudsToy Icon (it's a button that opens the CloudsToy info window!!!!)
    private void DrawCloudsToyIcon()
    {
        Rect iconRect = new Rect(0, 0, 500, 200);
        GUI.DrawTexture(iconRect, myCloudsToyIcon);
        GUILayout.Space(220);
    }

    private void DrawSpaces(int numSpaces) { for(int i = 0; i < numSpaces; i++) { EditorGUILayout.Space(); } }
    
}