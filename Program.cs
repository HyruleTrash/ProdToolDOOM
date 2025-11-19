using Gum.Forms.Controls;
using MonoGameGum;

namespace ProdToolDOOM;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

class Program : Game
{
    private GraphicsDeviceManager graphics;

    private GumService Gum => GumService.Default;
    
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
        Gum.Initialize(this);
        LoadUI();
        base.Initialize();
    }

    private void LoadUI()
    {
        var mainPanel = new StackPanel();
        mainPanel.AddToRoot();
        
        var exitButton = new Button();
        exitButton.Text = "Exit";
        exitButton.Click += (sender, args) => Exit();
        
        mainPanel.AddChild(exitButton);
    }
    
    protected override void LoadContent()
    {
        // _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Gum.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        Gum.Draw();
        base.Draw(gameTime);
    }
}