using UnityEngine;
using UnityEditor;
using System.Linq;

[InitializeOnLoad]
public class RenameEditor_cure {

    static RenameEditor_cure() {
        EditorApplication.hierarchyWindowItemOnGUI += hierarchyWindowItemCallback;
        EditorApplication.projectWindowItemOnGUI += ProjectWindowItemCallback;
    }

    private enum CHAGNE_TYPE {
        UPPER,
        LOWER,
        UPPER_ALL,
        LOWER_ALL,
        NUMBERING
    }

    private static string mousePointObjectName;
    private static bool _isNumberingZreo;
    private static bool _isNumberString;

    private static void hierarchyWindowItemCallback( int instanceID , Rect selectionRect ) {
        if( Event.current.type == EventType.MouseDown
              && Event.current.button == 1
              && selectionRect.Contains( Event.current.mousePosition ) ) {
            mousePointObjectName = EditorUtility.InstanceIDToObject( instanceID ).name;
        }
    }

    private static void ProjectWindowItemCallback( string guid , Rect selectionRect ) {
        if( Event.current.type == EventType.MouseDown
              && Event.current.button == 1
              && selectionRect.Contains( Event.current.mousePosition ) ) {
            string path = AssetDatabase.GUIDToAssetPath( guid );
            mousePointObjectName = AssetDatabase.LoadMainAssetAtPath( path ).name;
        }
    }


    [MenuItem( "Assets/RenameWord/ToUpper" , false , 0 )]
    [MenuItem( "GameObject/RenameWord/ToUpper" , false , 0 )]
    private static void FirstWordToUpper() {
        WordChange( CHAGNE_TYPE.UPPER );
    }

    [MenuItem( "Assets/RenameWord/ToLower" , false , 1 )]
    [MenuItem( "GameObject/RenameWord/ToLower" , false , 1 )]
    private static void FirstWordToLower() {
        WordChange( CHAGNE_TYPE.LOWER );
    }

    [MenuItem( "Assets/RenameWord/" , false , 2 )]
    [MenuItem( "GameObject/RenameWord/" , false , 2 )]
    [MenuItem( "Assets/RenameWord/AllToUpper" , false , 3 )]
    [MenuItem( "GameObject/RenameWord/AllToUpper" , false , 3 )]
    private static void AllWordToUpper() {
        WordChange( CHAGNE_TYPE.UPPER_ALL );
    }

    [MenuItem( "Assets/RenameWord/AllToLower" , false , 3 )]
    [MenuItem( "GameObject/RenameWord/AllToLower" , false , 3 )]
    private static void AllWordToLower() {
        WordChange( CHAGNE_TYPE.LOWER_ALL );
    }


    [MenuItem( "Assets/RenameWord/" , false , 4 )]
    [MenuItem( "GameObject/RenameWord/" , false , 4 )]
    [MenuItem( "Assets/RenameWord/Numbering" , false , 5 )]
    [MenuItem( "GameObject/RenameWord/Numbering" , false , 5 )]
    private static void WordNumbering() {
        if( string.IsNullOrEmpty( mousePointObjectName ) ) {
            return;
        }
        _isNumberString = false;
        WordChange( CHAGNE_TYPE.NUMBERING );
    }

    [MenuItem( "Assets/RenameWord/NumberString" , false , 6 )]
    [MenuItem( "GameObject/RenameWord/NumberString" , false , 6 )]
    private static void WordNumberString() {
        if( string.IsNullOrEmpty( mousePointObjectName ) ) {
            return;
        }
        _isNumberString = true;
        WordChange( CHAGNE_TYPE.NUMBERING  );
    }

    private static void WordChange( CHAGNE_TYPE type ) {
        if( Selection.objects == null ) {
            return;
        }
        if( Selection.objects[ 0 ] == null ) {
            return;
        }
        Object[] projectObjects = Selection.objects.Where( o => AssetDatabase.Contains( o ) )
                                             .OrderBy( o => o.name).ToArray();
        Object[] hierarchyObjects = Selection.gameObjects.Where( o => !AssetDatabase.Contains( o ) )
                                             .OrderBy( o => o.transform.GetSiblingIndex() ).ToArray();
        WordChangeArray( projectObjects , type , false);
        WordChangeArray( hierarchyObjects , type , true);
    }

    private static void WordChangeArray( Object[] objects , CHAGNE_TYPE type , bool isRecord ) {
        int count = 0;
        if( isRecord == true ) {
            Undo.RecordObjects(  objects , "Rename" );
        }
        foreach( Object obj in objects ) {
            if( string.IsNullOrEmpty( obj.name ) ) {
                Debug.Log( "Object Name is Null or Empty" );
                continue;
            }
            string name = null;
            switch( type ) {
                case CHAGNE_TYPE.UPPER:
                case CHAGNE_TYPE.LOWER:
                name = GetFirstWord( obj.name , type );
                break;
                case CHAGNE_TYPE.UPPER_ALL:
                name = obj.name.ToUpper();
                break;
                case CHAGNE_TYPE.LOWER_ALL:
                name = obj.name.ToLower();
                break;
                case CHAGNE_TYPE.NUMBERING:
                name = GetNumbering( count );
                count++;
                break;
            }
            if( AssetDatabase.Contains( obj ) ) {
                AssetDatabase.RenameAsset( AssetDatabase.GetAssetPath( obj ) , name );
            } else {
                obj.name = name;
            }
        }
    }
    private static string GetNumbering( int count ) {
        if( _isNumberString ) {
            return mousePointObjectName + "_" + NumberToText( count );
        }
        return mousePointObjectName + "_" + count;
    }

    private static string GetFirstWord( string word , CHAGNE_TYPE type ) {
        string firstWord = null;
        switch( type ) {
            case CHAGNE_TYPE.UPPER:
            firstWord = word.Substring( 0 , 1 ).ToUpper();
            break;
            case CHAGNE_TYPE.LOWER:
            firstWord = word.Substring( 0 , 1 ).ToLower();
            break;
        }
        word = word.Substring( 1 , word.Length - 1 );
        word = firstWord + word;
        return word;
    }

    public static string NumberToText( int n ) {
        if( n < 0 ) {
            return "Minus " + NumberToText( -n );
        } else if( n == 0 ) {
            return "Zero";
        } else if( n <= 19 ) {
            return new string[] {"One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight",
         "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen",
         "Seventeen", "Eighteen", "Nineteen"}[ n - 1 ] + " ";
        } else if( n <= 99 ) {
            return new string[] {"Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy",
         "Eighty", "Ninety"}[ n / 10 - 2 ] + " " + NumberToText( n % 10 );
        } else if( n <= 199 ) {
            return "One Hundred " + NumberToText( n % 100 );
        } else if( n <= 999 ) {
            return NumberToText( n / 100 ) + "Hundreds " + NumberToText( n % 100 );
        } else if( n <= 1999 ) {
            return "One Thousand " + NumberToText( n % 1000 );
        } else if( n <= 999999 ) {
            return NumberToText( n / 1000 ) + "Thousands " + NumberToText( n % 1000 );
        } else if( n <= 1999999 ) {
            return "One Million " + NumberToText( n % 1000000 );
        } else if( n <= 999999999 ) {
            return NumberToText( n / 1000000 ) + "Millions " + NumberToText( n % 1000000 );
        } else if( n <= 1999999999 ) {
            return "One Billion " + NumberToText( n % 1000000000 );
        } else {
            return NumberToText( n / 1000000000 ) + "Billions " + NumberToText( n % 1000000000 );
        }
    }

}

