using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TranslationModel
{
	/// <summary>
	/// ゲームメインクラス
	/// </summary>
	public class Game1 : Game
	{
    /// <summary>
    /// グラフィックデバイス管理クラス
    /// </summary>
    private readonly GraphicsDeviceManager _graphics = null;

    /// <summary>
    /// スプライトのバッチ化クラス
    /// </summary>
    private SpriteBatch _spriteBatch = null;

    /// <summary>
    /// スプライトでテキストを描画するためのフォント
    /// </summary>
    private SpriteFont _font = null;

    /// <summary>
    /// モデル
    /// </summary>
    private Model _model = null;

    /// <summary>
    /// モデルの位置
    /// </summary>
    private Vector3 _position = Vector3.Zero;

    /// <summary>
    /// 自動動作フラグ
    /// </summary>
    private int _autoMode = 0;

    /// <summary>
    /// マウスボタン押下フラグ
    /// </summary>
    private bool _isMousePressed = false;


    /// <summary>
    /// GameMain コンストラクタ
    /// </summary>
    public Game1()
    {
      // グラフィックデバイス管理クラスの作成
      _graphics = new GraphicsDeviceManager(this);

      // ゲームコンテンツのルートディレクトリを設定
      Content.RootDirectory = "Content";

      // マウスカーソルを表示
      IsMouseVisible = true;
    }

    /// <summary>
    /// ゲームが始まる前の初期化処理を行うメソッド
    /// グラフィック以外のデータの読み込み、コンポーネントの初期化を行う
    /// </summary>
    protected override void Initialize()
    {
      // TODO: ここに初期化ロジックを書いてください

      // コンポーネントの初期化などを行います
      base.Initialize();
    }

    /// <summary>
    /// ゲームが始まるときに一回だけ呼ばれ
    /// すべてのゲームコンテンツを読み込みます
    /// </summary>
    protected override void LoadContent()
    {
      // テクスチャーを描画するためのスプライトバッチクラスを作成します
      _spriteBatch = new SpriteBatch(GraphicsDevice);

      // フォントをコンテンツパイプラインから読み込む
      _font = Content.Load<SpriteFont>("Font");

      // モデルを作成
      _model = Content.Load<Model>("Model");

      // 今回はパラメータをあらかじめ設定しておく
      foreach (ModelMesh mesh in _model.Meshes)
      {
        foreach (BasicEffect effect in mesh.Effects)
        {
          // デフォルトのライト適用
          effect.EnableDefaultLighting();

          // ビューマトリックスをあらかじめ設定
          effect.View = Matrix.CreateLookAt(
              new Vector3(0.0f, 20.0f, 2.0f),
              Vector3.Zero,
              Vector3.Up
          );

          // プロジェクションマトリックスをあらかじめ設定
          effect.Projection = Matrix.CreatePerspectiveFieldOfView(
              MathHelper.ToRadians(45.0f),
              (float)GraphicsDevice.Viewport.Width /
                  (float)GraphicsDevice.Viewport.Height,
              1.0f,
              100.0f
          );
        }
      }
    }

    /// <summary>
    /// ゲームが終了するときに一回だけ呼ばれ
    /// すべてのゲームコンテンツをアンロードします
    /// </summary>
    protected override void UnloadContent()
    {
      // TODO: ContentManager で管理されていないコンテンツを
      //       ここでアンロードしてください
    }

    /// <summary>
    /// 描画以外のデータ更新等の処理を行うメソッド
    /// 主に入力処理、衝突判定などの物理計算、オーディオの再生など
    /// </summary>
    /// <param name="gameTime">このメソッドが呼ばれたときのゲーム時間</param>
    protected override void Update(GameTime gameTime)
    {
      KeyboardState keyState = Keyboard.GetState();
      MouseState mouseState = Mouse.GetState();
      GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

      // ゲームパッドの Back ボタン、またはキーボードの Esc キーを押したときにゲームを終了させます
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      {
        Exit();
      }

      // マウスによる自動動作切り替え
      if (_isMousePressed == false &&
          mouseState.LeftButton == ButtonState.Pressed)
      {
        _isMousePressed = true;

        _autoMode = (_autoMode + 1) % 3;
      }
      _isMousePressed = mouseState.LeftButton == ButtonState.Pressed;

      // キーボードによるモデルの移動操作
      if (keyState.IsKeyDown(Keys.Left))
      {
        _position.X -= 0.1f;
      }
      if (keyState.IsKeyDown(Keys.Right))
      {
        _position.X += 0.1f;
      }
      if (keyState.IsKeyDown(Keys.Up))
      {
        _position.Z -= 0.1f;
      }
      if (keyState.IsKeyDown(Keys.Down))
      {
        _position.Z += 0.1f;
      }
      if (_autoMode == 1)
      {
        _position.X = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * 4.0f;
      }
      if (_autoMode == 2)
      {
        _position.Z = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * 4.0f;
      }

      // ゲームパッドによるモデルの移動操作
      _position.X += gamePadState.ThumbSticks.Left.X * 0.1f;
      _position.Z -= gamePadState.ThumbSticks.Left.Y * 0.1f;

      // 登録された GameComponent を更新する
      base.Update(gameTime);
    }

    /// <summary>
    /// 描画処理を行うメソッド
    /// </summary>
    /// <param name="gameTime">このメソッドが呼ばれたときのゲーム時間</param>
    protected override void Draw(GameTime gameTime)
    {
      // 画面を指定した色でクリアします
      GraphicsDevice.Clear(Color.CornflowerBlue);

      foreach (ModelMesh mesh in _model.Meshes)
      {
        // モデルの位置を設定
        foreach (BasicEffect effect in mesh.Effects)
        {
          effect.World = Matrix.CreateTranslation(_position);
        }

        // モデルを描画
        mesh.Draw();
      }

      // スプライトの描画準備
      _spriteBatch.Begin();

      // テキストをスプライトとして描画する
      _spriteBatch.DrawString(_font,
          "X:" + _position.X.ToString() + Environment.NewLine +
          "Y:" + _position.Y.ToString() + Environment.NewLine +
          "Z:" + _position.Z.ToString() + Environment.NewLine +
          "MousePressAutoMode : " + _autoMode,
          new Vector2(50, 50), Color.White);

      // スプライトの一括描画
      _spriteBatch.End();

      // 登録された DrawableGameComponent を描画する
      base.Draw(gameTime);
    }
  }
}
