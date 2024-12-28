
using System.Runtime.InteropServices;

///-----------------------------------
///ファイル名変更プログラム
///-----------------------------------
try
{
    Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■");
    Console.WriteLine("ファイル名変更プログラム");
    Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■");
    Console.WriteLine("");

    //コマンドライン引数を配列で取得する
    string[] cmds = System.Environment.GetCommandLineArgs();
    if (cmds.Length <= 3)
    {
        throw new Exception("引数が足りません。\n[対象パス] [ワイルドカード] [ファイルプレフィックス] ([保存パス] [フォルダモード])");
    }

    // 対象パスの取得
    string tgtPath = cmds[1];
    Console.WriteLine($"対象パス :{tgtPath} ");
    if (!System.IO.Directory.Exists(tgtPath))
    {
        throw new Exception($"対象パスが存在しません。");
    }

    //ワイルドカード
    string wildCard = cmds[2];
    if(string.IsNullOrEmpty(wildCard))
    {
        wildCard = "*.*";
    }
    Console.WriteLine($"ワイルドカード :{wildCard} ");

    //ファイル名の前半部分
    string pefix = cmds[3];
    Console.WriteLine($"プレフィックス :{pefix} ");

    //フォルダ移動モード指定
    // 0 : 移動なし
    // 1 : 年月日単位のフォルダに移動
    int mode = 0;
    string savePath = "";
    if(cmds.Length >= 5)
    {
        string m = cmds[5];
        int.TryParse(m, out mode);
        Console.WriteLine($"フォルダ移動モード :{mode} ");

        savePath = cmds[4];
        Console.WriteLine($"保存パス :{savePath} ");
        if (!System.IO.Directory.Exists(savePath))
        {
            throw new Exception($"保存パスが存在しません。");
        }
    }


    Console.WriteLine("");


    //ファイル取得
    string[] files = Directory.GetFiles(tgtPath, wildCard, SearchOption.TopDirectoryOnly);
    foreach (string file in files)
    {

        string filename = Path.GetFileName(file);
        string? filePath = Path.GetDirectoryName(file);
        if(filePath == null || string.IsNullOrWhiteSpace(filename))
        {
            continue;
        }

        //最終更新日時を取得
        DateTime fileTime = File.GetLastWriteTime(file);

        string newFilename;
        string newFilePath;
        do
        {
            //新しいファイル名を生成
            newFilename = $"{pefix}_{fileTime.ToString("yyyyMMdd_HHmmss")}{Path.GetExtension(file)}";


            if(mode == 1)
            {
                //保存先
                string savePathYear = fileTime.ToString("yyyy");
                string savePathMonth = fileTime.ToString("MM");
                string savePathday = fileTime.ToString("dd");

                //年のフォルダ作成
                string saveFillPath = savePath;
                saveFillPath = Path.Combine(saveFillPath, savePathYear);
                if (!Directory.Exists(saveFillPath))
                {
                    Directory.CreateDirectory(saveFillPath);
                }

                //月のフォルダ作成
                saveFillPath = Path.Combine(saveFillPath, savePathMonth);
                if (!Directory.Exists(saveFillPath))
                {
                    Directory.CreateDirectory(saveFillPath);
                }

                //日のフォルダ作成
                saveFillPath = Path.Combine(saveFillPath, savePathday);
                if (!Directory.Exists(saveFillPath))
                {
                    Directory.CreateDirectory(saveFillPath);
                }

                newFilePath = Path.Combine(saveFillPath, newFilename);
            }
            else
            {
                //同一階層
                newFilePath = Path.Combine(filePath, newFilename);

                //新旧ファイル名が同じかチェック
                if (filename.ToUpper() == newFilename.ToUpper())
                {
                    //同じ名前は何もしない
                    break;
                }
            }

            //同名が存在するかチェック
            if (!File.Exists(newFilePath))
            {
                //ファイル名を変更
                File.Move(file, newFilePath);
                Console.WriteLine($"[{filename}] → [{newFilename}]");
                break;
            }

            fileTime = fileTime.AddSeconds(1);

        } while (true);




    }

    Console.WriteLine($"終了しました。");
}
catch (Exception ex)
{
    Console.WriteLine("ERROR ---------------------------------");
    Console.WriteLine(ex.Message);
    Console.WriteLine("--------------------------------- ERROR");
}






