
using System.Runtime.InteropServices;

///-----------------------------------
///ファイル名変更＆フォルダ仕分けプログラム
///-----------------------------------
try
{
    Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■");
    Console.WriteLine("ファイル名変更＆フォルダ仕分けプログラム");
    Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■");
    Console.WriteLine("");

    //コマンドライン引数を配列で取得する
    string[] cmds = System.Environment.GetCommandLineArgs();
    if (cmds.Length <= 4)
    {
        throw new Exception("引数が足りません。\n[対象パス] [対象ファイルのワイルドカード] [変更後のファイルプレフィックス] [別フォルダに移動させる日数]");
    }

    // 対象パスの取得
    string tgtPath = cmds[1];
    Console.WriteLine($"対象パス :{tgtPath} ");
    if (!System.IO.Directory.Exists(tgtPath))
    {
        throw new Exception($"対象パスが存在しません。");
    }

    //対象ファイルのワイルドカード
    string wildCard = cmds[2];
    if (string.IsNullOrEmpty(wildCard))
    {
        wildCard = "*.*";
    }
    Console.WriteLine($"ワイルドカード :{wildCard} ");


    //ファイル名の前半部分
    string pefix = cmds[3];
    Console.WriteLine($"プレフィックス :{pefix} ");
    Console.WriteLine("");


    //移動させない日数
    int notMovedays = 0;
    int.TryParse(cmds[4], out notMovedays);
    Console.WriteLine($"移動させない日数 :{notMovedays.ToString()} ");
    Console.WriteLine("");


    //ファイル取得
    string[] files = Directory.GetFiles(tgtPath, wildCard, SearchOption.TopDirectoryOnly);
    foreach (string file in files)
    {
        string filename = Path.GetFileName(file);
        string? filePath = Path.GetDirectoryName(file);
        if (filePath == null || string.IsNullOrWhiteSpace(filename))
        {
            //あり得ないが処理しない
            continue;
        }
        if (filename.ToLower() == "desktop.ini".ToLower())
        {
            continue;
        }




        //最終更新日時を取得
        DateTime fileTime = File.GetLastWriteTime(file);

        string newFilename;
        string newFilePath;
        int suffixus = 0;
        do//同名が存在する場合の対策（存在しなくなるまでループ）
        {
            //新しいファイル名を生成
            if (pefix != "*")
            {
                newFilename = $"{pefix}_{fileTime.ToString("yyyyMMdd_HHmmss")}{Path.GetExtension(file)}";
            }
            else
            {
                //変更しない
                newFilename = filename;
                if(suffixus > 0)
                {
                    //同名があるので、最後に数値をつける
                    newFilename = Path.GetFileNameWithoutExtension(filename) + $"-{suffixus.ToString()}." + Path.GetExtension(filename);
                }
            }

            //指定日数以上はサブフォルダに移動させる
            DateTime checkTime = DateTime.Now.AddDays(notMovedays * -1);
            string moveFolder = "";
            if (notMovedays <= 0 || checkTime <= fileTime)
            {
                //移動させない

                //同一階層
                newFilePath = Path.Combine(filePath, newFilename);

                //新旧ファイル名が同じかチェック
                if (filename.ToUpper() == newFilename.ToUpper())
                {
                    //同じ名前は何もしない
                    break;
                }
            }
            else
            {
                //保存先
                string savePathYear = fileTime.ToString("yyyy年");
                string savePathMonth = fileTime.ToString("yyyy年MM月");
                string savePathday = fileTime.ToString("yyyy年MM月dd日");

                //年のフォルダ作成
                string saveFillPath = tgtPath;
                saveFillPath = Path.Combine(saveFillPath, savePathYear);
                moveFolder += $"{savePathYear}";
                if (!Directory.Exists(saveFillPath))
                {
                    Directory.CreateDirectory(saveFillPath);
                }

                //月のフォルダ作成
                saveFillPath = Path.Combine(saveFillPath, savePathMonth);
                moveFolder += $"\\{savePathMonth}";
                if (!Directory.Exists(saveFillPath))
                {
                    Directory.CreateDirectory(saveFillPath);
                }

                //移動先パス＆新しいファイル名
                newFilePath = Path.Combine(saveFillPath, newFilename);
            }

            //同名が存在するかチェックし、なければ名前変更及びフォルダ移動
            if (!File.Exists(newFilePath))
            {
                //ファイル名を変更
                File.Move(file, newFilePath);

                if (moveFolder == "")
                {
                    //ファイル名変更のみ
                    Console.WriteLine($"[{filename}] → [{newFilename}]");
                }
                else
                {
                    //フォルダ移動+ファイル名変更
                    Console.WriteLine($"[{filename}] → [{moveFolder}\\{newFilename}]");
                }
                break;
            }

            //存在するので、1秒進めたファイル名する
            fileTime = fileTime.AddSeconds(1);
            suffixus++;

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






