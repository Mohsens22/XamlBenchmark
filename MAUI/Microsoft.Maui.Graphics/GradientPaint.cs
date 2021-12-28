﻿using System;

namespace Microsoft.Maui.Graphics
{
	public abstract class GradientPaint : Paint
	{
		GradientStop[] _gradientStops =
		{
			new GradientStop(0, Colors.White),
			new GradientStop(1, Colors.White)
		};

		public GradientPaint()
		{
		}

		public GradientPaint(GradientPaint source)
		{
			if (source != null)
			{
				_gradientStops = new GradientStop[source.GradientStops.Length];

				for (var i = 0; i < _gradientStops.Length; i++)
					_gradientStops[i] = new GradientStop(source.GradientStops[i]);	
			}
		}

		public GradientStop[] GradientStops
		{
			get => _gradientStops;
			set
			{
				_gradientStops = value;

				if (_gradientStops == null || _gradientStops.Length == 0)
					_gradientStops = new[] { new GradientStop(0, Colors.White), new GradientStop(1, Colors.White) };
			}
		}

		public Color StartColor
		{
			get => _gradientStops[StartColorIndex].Color;
			set
			{
				var startColorIndex = StartColorIndex;
				_gradientStops[startColorIndex].Color = value ?? Colors.White;
			}
		}

		public Color EndColor
		{
			get => _gradientStops[EndColorIndex].Color;
			set
			{
				var endColorIndex = EndColorIndex;
				_gradientStops[endColorIndex].Color = value ?? Colors.White;
			}
		}

		public int StartColorIndex
		{
			get
			{
				var index = -1;
				float offset = 1;

				for (var i = 0; i < _gradientStops.Length; i++)
				{
					if (_gradientStops[i].Offset <= offset)
					{
						index = i;
						offset = _gradientStops[i].Offset;
					}
				}

				return index >= 0 ? index : 0;
			}
		}

		public int EndColorIndex
		{
			get
			{
				var index = -1;
				float offset = 0;

				for (var i = 0; i < _gradientStops.Length; i++)
				{
					if (_gradientStops[i].Offset >= offset)
					{
						index = i;
						offset = _gradientStops[i].Offset;
					}
				}

				return index >= 0 ? index : _gradientStops.Length - 1;
			}
		}

		public override bool IsTransparent
		{
			get
			{
				foreach (var stop in GradientStops)
				{
					if (stop.Color != null && stop.Color.Alpha < 1)
					{
						return true;
					}
				}

				return false;
			}
		}

		public GradientStop[] GetSortedStops()
		{
			var vStops = new GradientStop[_gradientStops.Length];
			Array.Copy(_gradientStops, vStops, _gradientStops.Length);
			Array.Sort(vStops);

			return vStops;
		}

		public void SetGradientStops(float[] offsets, Color[] colors)
		{
			var stopCount = Math.Min(colors.Length, offsets.Length);
			_gradientStops = new GradientStop[stopCount];

			for (var p = 0; p < stopCount; p++)
			{
				_gradientStops[p] = new GradientStop(offsets[p], colors[p]);
			}
		}

		public void AddOffset(float offset)
		{
			AddOffset(offset, GetColorAt(offset));
		}

		public void AddOffset(float offset, Color color)
		{
			var oldStops = GradientStops;
			var newStops = new GradientStop[oldStops.Length + 1];

			for (var i = 0; i < oldStops.Length; i++)
				newStops[i] = oldStops[i];

			newStops[oldStops.Length] = new GradientStop(offset, color);

			GradientStops = newStops;
		}

		public void RemoveOffset(int index)
		{
			if (index < 0 || index >= GradientStops.Length)
			{
				return;
			}

			var oldStops = GradientStops;
			var newStops = new GradientStop[oldStops.Length - 1];
			for (var i = 0; i < oldStops.Length; i++)
			{
				if (i < index)
				{
					newStops[i] = oldStops[i];
				}
				else if (i > index)
				{
					newStops[i - 1] = oldStops[i];
				}
			}

			GradientStops = newStops;
		}

		public Color GetColorAt(float offset)
		{
			var stops = GradientStops
				;
			if (stops.Length == 1)
			{
				return stops[0].Color;
			}

			var before = float.MaxValue;
			var beforeIndex = -1;
			var after = float.MaxValue;
			var afterIndex = -1;

			for (var i = 0; i < stops.Length; i++)
			{
				var currentOffset = stops[i].Offset;

				if (Math.Abs(currentOffset - offset) < Geometry.Epsilon)
				{
					return stops[i].Color;
				}

				if (currentOffset < offset)
				{
					var dx = offset - currentOffset;
					if (dx < before)
					{
						before = currentOffset;
						beforeIndex = i;
					}
				}
				else if (currentOffset > offset)
				{
					var dx = currentOffset - offset;
					if (dx < after)
					{
						after = currentOffset;
						afterIndex = i;
					}
				}
			}

			if (afterIndex == -1)
			{
				return EndColor;
			}

			if (beforeIndex == -1)
			{
				return StartColor;
			}

			var f = Geometry.GetFactor(before, after, offset);
			return BlendStartAndEndColors(stops[beforeIndex].Color, stops[afterIndex].Color, f);
		}

		public Color BlendStartAndEndColors()
		{
			if (_gradientStops == null || _gradientStops.Length < 2)
			{
				return Colors.White;
			}

			return BlendStartAndEndColors(StartColor, EndColor, .5f);
		}

		public Color BlendStartAndEndColors(Color startColor, Color endColor, float factor)
		{
			startColor ??= Colors.White;
			endColor ??= Colors.White;

			var r = Geometry.GetLinearValue(startColor.Red, endColor.Red, factor);
			var g = Geometry.GetLinearValue(startColor.Green, endColor.Green, factor);
			var b = Geometry.GetLinearValue(startColor.Blue, endColor.Blue, factor);
			var a = Geometry.GetLinearValue(startColor.Alpha, endColor.Alpha, factor);

			return new Color(r, g, b, a);
		}

		public override string ToString()
		{
			return $"[{nameof(GradientPaint)}: StartColor={StartColor}, EndColor={EndColor}]";
		}
	}
}
