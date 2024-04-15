﻿namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;

/// <summary>
/// Represents a animation that can be played in console
/// </summary>
public sealed class NexusAnimation
{
    private readonly ReadOnlyMemory<NexusImage> _images;

    private int currentFrameIndex;

    /// <summary>
    /// The width of the animation
    /// </summary>
    public int Width => _images.Span[0].Width;

    /// <summary>
    /// The height of the animation
    /// </summary>
    public int Height => _images.Span[0].Height;

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="filepath">The path to the gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    public NexusAnimation(string filepath, NexusImageProcessor imageProcessor)
        : this(filepath, imageProcessor, Size.Empty) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="animation">The gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    public NexusAnimation(Bitmap animation, NexusImageProcessor imageProcessor)
        : this(animation, imageProcessor, Size.Empty) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="filepath">The path to the gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="percentage">The desired percentage size of the bitmap</param>
    public NexusAnimation(string filepath, NexusImageProcessor imageProcessor, float percentage)
        : this(new Bitmap(filepath), imageProcessor, percentage) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="animation">The gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="percentage">The desired percentage size of the bitmap</param>
    public NexusAnimation(Bitmap animation, NexusImageProcessor imageProcessor, float percentage)
    {
        if (animation.RawFormat.Guid != ImageFormat.Gif.Guid)
            throw new ArgumentException("The file has to be a gif file");

        _images = Initialize(animation, imageProcessor, ImageHelper.GetSize(animation.Width, animation.Height, percentage));

        currentFrameIndex = -1;
    }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="filepath">The path to the gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="size">The desired size of the animation</param>
    public NexusAnimation(string filepath, NexusImageProcessor imageProcessor, Size size)
        : this(new Bitmap(filepath), imageProcessor, size) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="animation">The gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="size">The desired size of the animation</param>
    public NexusAnimation(Bitmap animation, NexusImageProcessor imageProcessor, Size size)
    {
        if (animation.RawFormat.Guid != ImageFormat.Gif.Guid)
            throw new ArgumentException("The file has to be a gif file");

        _images = Initialize(animation, imageProcessor, size);

        currentFrameIndex = -1;
    }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="images">The images the animation should have</param>
    public NexusAnimation(params NexusImage[] images)
    {
        if (images.Length is 0) throw new ArgumentException("The images should be at least 1");

        _images = new ReadOnlyMemory<NexusImage>(images);

        currentFrameIndex = -1;
    }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="images">The images the animation should have</param>
    public NexusAnimation(in ReadOnlySpan<NexusImage> images) : this(images.ToArray()) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="url">The url of the gif</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <returns><see cref="NexusAnimation"/></returns>
    /// <exception cref="HttpRequestException"></exception>
    public static NexusAnimation FromUrl(Uri url, NexusImageProcessor imageProcessor)
    {
        using (var client = new HttpClient())
        {
            using (var stream = TaskHelper.RunSync(() => client.GetStreamAsync(url)))
            {
                return stream is null
                    ? throw new HttpRequestException("Couldn't read the image")
                    : new NexusAnimation(new Bitmap(stream), imageProcessor);
            }
        }
    }

    internal NexusImage NextFrame()
    {
        currentFrameIndex++;

        if (currentFrameIndex == _images.Length) currentFrameIndex = 0;

        return _images.Span[currentFrameIndex];
    }

    private static ReadOnlyMemory<NexusImage> Initialize(Bitmap bitmap, NexusImageProcessor processor, Size size)
    {
        var dimension = new FrameDimension(bitmap.FrameDimensionsList[0]);
        var frames = bitmap.GetFrameCount(dimension);

        var images = new NexusImage[frames];
        for (int i = 0; i < frames; i++)
        {
            bitmap.SelectActiveFrame(dimension, i);

            images[i] = new NexusImage(bitmap, processor, size);
        }
        
        return new ReadOnlyMemory<NexusImage>(images);
    }
}