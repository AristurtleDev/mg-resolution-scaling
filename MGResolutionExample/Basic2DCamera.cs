using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MGResolutionExample;

public sealed class Basic2DCamera
{
    //  The transformation matrix of the camera.
    private Matrix _transformationMatrix;

    //  The inverse of the transformation matrix.
    private Matrix _inverseMatrix;

    //  The xy-coordinate position of the camera.
    private Vector2 _position;

    //  The rotation of the camera, in radians
    private float _rotation;

    //  The zoom level of the camera on the x and y axes
    private Vector2 _zoom;

    //  The origin point of the camera.
    private Vector2 _origin;

    //  A value that indicates if the position, rotation, zoom, and/or origin
    //  values of the camera have changed, which means the matrices need to
    //  be updated
    private bool _hasChanged;

    //  Describes the viewing bounds of the camera.
    public Viewport Viewport { get; set; }

    /// <summary>
    ///     Gets a <see cref="Matrix"/> value that describes the position,
    ///     rotation, zoom, and origin of this <see cref="Camera"/>.
    /// </summary>
    public Matrix TransformationMatrix
    {
        get
        {
            //  Check if a change has been made and update first before
            //  returning a value.
            if (_hasChanged)
            {
                UpdateMatrices();
            }

            return _transformationMatrix;
        }
    }

    /// <summary>
    ///     Gets a <see cref="Matrix"/> value that is the inverse of the
    ///     <see cref="TransformationMatrix"/> value.
    /// </summary>
    public Matrix InverseMatrix
    {
        get
        {
            //  Check if a change has been made and update first before
            //  returning a value.
            if (_hasChanged)
            {
                UpdateMatrices();
            }

            return _inverseMatrix;
        }
    }

    /// <summary>
    ///     Gets or Sets a <see cref="Vector2"/> value representing the
    ///     xy-coordinate position of this <see cref="Camera"/> in world space.
    /// </summary>
    public Vector2 Position
    {
        get { return _position; }
        set
        {
            //  Only set value if it has actually changed.
            if (_position == value) { return; }

            _position = value;
            _hasChanged = true;
        }
    }

    /// <summary>
    ///     Gets or Sets a <see cref="float"/> value representing the
    ///     x-coordinate position of this <see cref="Camera"/> in world space.
    /// </summary>
    public float X
    {
        get { return _position.X; }
        set
        {
            //  Only set value if it has actually changed.
            if (_position.X == value) { return; }

            _position.X = value;
            _hasChanged = true;
        }
    }

    /// <summary>
    ///     Gets or Sets a <see cref="float"/> value representing the
    ///     y-coordinate position of this <see cref="Camera"/> in world space.
    /// </summary>
    public float Y
    {
        get { return _position.Y; }
        set
        {
            //  Only set value if it has actually changed.
            if (_position.Y == value) { return; }

            _position.Y = value;
            _hasChanged = true;
        }
    }

    /// <summary>
    ///     Gets or Sets a <see cref="float"/> value that represents the rotation
    ///     of this <see cref="Camera"/>, in radians.
    /// </summary>
    public float Rotation
    {
        get { return _rotation; }
        set
        {
            //  Only set value if it has actually changed.
            if (_rotation == value) { return; }

            _rotation = value;
            _hasChanged = true;
        }
    }

    /// <summary>
    ///     Gets or Sets a <see cref="Vector2"/> value that represents the zoom
    ///     level of this <see cref="Camera"/> on the x and y axes.
    /// </summary>
    public Vector2 Zoom
    {
        get { return _zoom; }
        set
        {
            //  Only set value if it has actually changed.
            if (_zoom == value) { return; }

            _zoom = value;
            _hasChanged = true;
        }
    }

    /// <summary>
    ///     Gets or Sets a <see cref="Vector2"/> value that represents the
    ///     xy-coordinate origin point of this <see cref="Camera"/>.
    /// </summary>
    public Vector2 Origin
    {
        get { return _origin; }
        set
        {
            //  Only set value if it has actually changed.
            if (_origin == value) { return; }

            _origin = value;
            _hasChanged = true;
        }
    }

    /// <summary>
    ///     Create a new <see cref="Camera"/> instance.
    /// </summary>
    /// <param name="width">
    ///     A <see cref="int"/> value that describes the width, in pixels,
    ///     of the viewport used by this <see cref="Camera"/>
    /// </param>
    /// <param name="height">
    ///     A <see cref="int"/> value that describes the height, in pixels,
    ///     of the viewport used by this <see cref="Camera"/>
    /// </param>
    public Basic2DCamera(int width, int height)
        : this(new Viewport(0, 0, width, height, 0, 1)) { }

    /// <summary>
    ///     Creates a new <see cref="Camera"/> instance.
    /// </summary>
    /// <param name="viewport">
    ///     A <see cref="Viewport"/> value that describes the viewport
    ///     used by this <see cref="Camera"/>.
    /// </param>
    public Basic2DCamera(Viewport viewport)
    {
        _position = Vector2.Zero;
        _rotation = 0.0f;
        _origin = Vector2.Zero;
        _zoom = Vector2.One;

        Viewport = viewport;
        UpdateMatrices();
    }

    /// <summary>
    ///     Updates the values for the <see cref="TransformationMatrix"/> and
    ///     <see cref="InverseMatrix"/> properties of this <see cref="Camera"/>.
    /// </summary>
    private void UpdateMatrices()
    {
        //  Create a translation matrix based on the position of the camera
        Matrix translationMatrix = Matrix.CreateTranslation(new Vector3
        {
            X = -(int)Math.Floor(_position.X),
            Y = -(int)Math.Floor(_position.Y),
            Z = 0
        });

        //  Create a rotation matrix based on the rotation of the camera
        Matrix rotationMatrix = Matrix.CreateRotationZ(_rotation);

        //  Create a scale matrix based on the zoom level of the camera
        Matrix scaleMatrix = Matrix.CreateScale(new Vector3
        {
            X = _zoom.X,
            Y = _zoom.Y,
            Z = 1
        });

        //  Create a translation matrix based on the origin point of the camera
        Matrix originTranslationMatrix = Matrix.CreateTranslation(new Vector3
        {
            X = (int)Math.Floor(_origin.X),
            Y = (int)Math.Floor(_origin.Y),
            Z = 0
        });

        //  Perform matrix multiplication of the matrices created above.
        //  Note: The order of multiplication is important
        _transformationMatrix = Matrix.Identity *
                                translationMatrix *
                                rotationMatrix *
                                scaleMatrix *
                                originTranslationMatrix;

        //  Get the inverse matrix
        _inverseMatrix = Matrix.Invert(_transformationMatrix);

        //  Since all matrices have been updates, set has changed to false.
        _hasChanged = false;
    }

    /// <summary>
    ///     Centers the <see cref="Origin"/> values of this <see cref="Camera"/> based
    ///     on the current <see cref="Position"/>.
    /// </summary>
    public void CenterOrigin()
    {
        Origin = new Vector2(Viewport.Width, Viewport.Height) * 0.5f;
    }

    /// <summary>
    ///     Given a <see cref="Vector2"/> value in screen space, translates it
    ///     to the equivalent <see cref="Vector2"/> value in camera/world space.
    /// </summary>
    /// <param name="screenPosition">
    ///     A <see cref="Vector2"/> value representing the xy-coordinate position
    ///     in screen space to translate.
    /// </param>
    /// <returns>
    ///     A <see cref="Vector2"/> value that is the camera/world space equivalent
    ///     of the screen space <see cref="Vector2"/> given.
    /// </returns>
    public Vector2 ScreenToCamera(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition, InverseMatrix);
    }

    /// <summary>
    ///     Given a <see cref="Vector2"/> value in camera/world space, translates it
    ///     to the equivalent <see cref="Vector2"/> value in screen space.
    /// </summary>
    /// <param name="screenPosition">
    ///     A <see cref="Vector2"/> value representing the xy-coordinate position
    ///     in camera/world space to translate.
    /// </param>
    /// <returns>
    ///     A <see cref="Vector2"/> value that is the screen space equivalent
    ///     of the camera/world space <see cref="Vector2"/> given.
    /// </returns>
    public Vector2 CameraToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, TransformationMatrix);
    }
}