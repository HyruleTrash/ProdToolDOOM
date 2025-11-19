namespace ProdToolDOOM;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

class Program : Game
{
    private GraphicsDeviceManager graphics;
    
    public Program()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    
    static void Main(string[] args)
    {
        Console.WriteLine("Starting application...");
        var p = new Program();
        p.Run();
    }
    
    protected override void Initialize()
    {
        base.Initialize();
    }
    
    protected override void LoadContent()
    {
        // _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }
}