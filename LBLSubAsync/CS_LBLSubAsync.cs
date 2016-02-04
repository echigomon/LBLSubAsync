using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LRSkipAsync;
using WsuppAsync;
using BufsupAsync;
using GetkenAsync;
using RsvwrdAsync;

namespace LBLSubAsync
{
    public class CS_LBLSubAsync
    {
        #region 共有領域
        CS_BufsupAsync bufsup;
        CS_WsuppAsync wsupp;
        CS_LRskipAsync lrskip;
        CS_GetkenAsync getken;
        CS_RsvwrdAsync rsvwrd;

        private static String _wbuf;       // ソース情報
        private static Boolean _empty;     // ソース情報有無
        private static String _rem;        // 注釈情報
        private static Boolean _remark;    // 注釈管理情報
        private static String[] _array;    // トークン抽出情報
        private static int _wcnt;          // トークン要素数
        public String Wbuf
        {
            get
            {
                return (_wbuf);
            }
            set
            {
                _wbuf = value;
                if (_wbuf == null)
                {   // 設定情報は無し？
                    _empty = true;
                }
                else
                {   // 整形処理を行う
                    // 不要情報削除
                    if (lrskip == null)
                    {   // 未定義？
                        lrskip = new CS_LRskipAsync();
                    }
                    lrskip.ExecAsync(_wbuf);
                    _wbuf = lrskip.Wbuf;

                    // 作業の為の下処理
                    if (_wbuf.Length == 0 || _wbuf == null)
                    {   // バッファー情報無し
                        // _wbuf = null;
                        _empty = true;
                    }
                    else
                    {
                        _empty = false;

                    }
                    _rem = null;
                }
            }
        }

        public String Rem
        {
            get
            {
                return (_rem);
            }
        }

        public Boolean Remark
        {
            get
            {
                return (_remark);
            }
            set
            {   // 16.01.28 連続呼び出し時の状況設定追加
                _remark = value;
            }
        }

        #endregion

        #region コンストラクタ
        public CS_LBLSubAsync()
        {   // コンストラクタ
            _wbuf = null;       // 設定情報無し
            _empty = true;
            _rem = null;
            _remark = false;

            wsupp = null;
            bufsup = null;
            getken = null;
            rsvwrd = null;
        }
        #endregion

        #region モジュール
        public async Task ClearAsync()
        {   // 作業領域の初期化
            _wbuf = null;       // 設定情報無し
            _empty = true;
            _rem = null;
            _remark = false;

            wsupp = null;
            bufsup = null;
            getken = null;
            rsvwrd = null;
        }
        public async Task ExecAsync()
        {   // ラベル評価を行う
            if (!_empty)
            {   // バッファーに実装有り
                await SetWsuppAsync(_wbuf);     // 引用符間情報のクリア

                await SetBufsupAsync(_wbuf, _remark);    // 構文評価を行う

                if (_wbuf != null)
                {   // 評価対象有り？
                    await SetGetkenAsync(_wbuf);    // トークン抽出を行う 

                    await SetRsvwrdAsync();     // 予約語確認を行う
                }
            }
        }
        public async Task ExecAsync(Boolean remflg)
        {   // ラベル評価を行う
            if (!_empty)
            {   // バッファーに実装有り
                await SetWsuppAsync(_wbuf);     // 引用符間情報のクリア

                await SetBufsupAsync(_wbuf, remflg);    // 構文評価を行う

                if (_wbuf != null)
                {   // 評価対象有り？
                    await SetGetkenAsync(_wbuf);    // トークン抽出を行う 

                    await SetRsvwrdAsync();     // 予約語確認を行う
                }
            }
        }
        public async Task ExecAsync(String msg)
        {   // ラベル評価を行う
            await SetbufAsync(msg);                 // 入力内容の作業領域設定

            if (!_empty)
            {   // バッファーに実装有り
            }
        }
        #endregion

        #region サブ・モジュール
        private async Task SetbufAsync(String _strbuf)
        {   // [_wbuf]情報設定
            _wbuf = _strbuf;
            if (_wbuf == null)
            {   // 設定情報は無し？
                _empty = true;
            }
            else
            {   // 整形処理を行う
                // 不要情報削除
                if (lrskip == null)
                {   // 未定義？
                    lrskip = new CS_LRskipAsync();
                }
                await lrskip.ExecAsync(_wbuf);
                _wbuf = lrskip.Wbuf;

                // 作業の為の下処理
                if (_wbuf.Length == 0 || _wbuf == null)
                {   // バッファー情報無し
                    // _wbuf = null;
                    _empty = true;
                }
                else
                {
                    _empty = false;
                }

                _rem = null;        // 注釈情報　初期化
            }
        }
        private async Task SetWsuppAsync(String _strbuf)
        {   // 引用符間情報のクリア
            if (wsupp == null)
            {   // 未定義？
                wsupp = new CS_WsuppAsync();
            }

            if (_strbuf != null)
            {   // 評価情報有り？
                await wsupp.ClearAsync();
                await wsupp.ExecAsync(_wbuf);

                _wbuf = wsupp.Wbuf;
            }
        }
        private async Task SetBufsupAsync(String _strbuf, Boolean __remark)
        {   // 構文評価を行う
            if (bufsup == null)
            {   // 未定義？
                bufsup = new CS_BufsupAsync();
            }

            if (_strbuf != null)
            {   // 評価有り？
                await bufsup.ClearAsync();
                bufsup.Remark = __remark;
                await bufsup.ExecAsync(_wbuf);

                _wbuf = bufsup.Wbuf;
                _rem = bufsup.Rem;
                _remark = bufsup.Remark;
            }
        }
        private async Task SetGetkenAsync(String _strbuf)
        {   // トークン抽出を行う
            if (getken == null)
            {   // 未定義？
                getken = new CS_GetkenAsync();
            }

            if (_strbuf != null)
            {   // 評価有り？
                await getken.ClearAsync();
                await getken.ExecAsync(_strbuf);
                _wcnt = getken.Wcnt;

                if (_wcnt != 0)
                {   // トークン有り？
                    _array = new String[_wcnt];
                    for(int i=0; i<_wcnt; i++)
                    {   // 登録情報に対して、処理を行う。
                        _array[i] = getken.Array[i];
                    }
                }
            }
        }
        private async Task SetRsvwrdAsync()
        {   // 予約語確認
            if (_wcnt != 0)
            {   // 評価対象有り？
                if (rsvwrd == null)
                {   // 未定義？
                    rsvwrd = new CS_RsvwrdAsync();
                }
                await rsvwrd.ClearAsync();

                for (int i = 0; i < _wcnt; i++)
                {   // 全てのトークンに対して処理を行う。
                    await rsvwrd.ExecAsync(_array[i]);      // 予約語確認を行う
                    if (rsvwrd.Rsv == false)
                    {   // 予約語未検出？
                        if (rsvwrd.Is_namespace)
                        {   // [namespace]検出？
                            // [namespace]対応処理
                            rsvwrd.Is_namespace = false;
                            break;
                        }
                        if (rsvwrd.Is_class)
                        {   // [class]検出？
                            // [class]対応処理
                            rsvwrd.Is_class = false;
                            break;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
