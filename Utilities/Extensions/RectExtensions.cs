//****************** 代码文件申明 ***********************
//* 文件：RectExtensions
//* 作者：wheat
//* 创建时间：2024/05/27 19:15:55 星期一
//* 描述：拓展Rect的一些方法
//*******************************************************

using UnityEngine;
using System;

namespace KFrame.Utilities
{
    public static class RectExtensions
    {
        public static Rect TakeFromLeft(this ref Rect rect, float width)
        {
            float num = Math.Min(rect.width, width);
            Rect result = rect;
            result.width = num;
            rect.x += num;
            rect.width -= num;
            return result;
        }

        public static Rect TakeFromRight(this ref Rect rect, float width)
        {
            float num = Math.Min(rect.width, width);
            Rect result = rect.AlignRight(num);
            rect.width -= num;
            return result;
        }

        public static Rect TakeFromTop(this ref Rect rect, float height)
        {
            float num = Math.Min(rect.height, height);
            Rect result = rect;
            result.height = num;
            rect.y += num;
            rect.height -= num;
            return result;
        }

        public static Rect TakeFromBottom(this ref Rect rect, float height)
        {
            float height2 = Math.Min(rect.height, height);
            Rect result = rect.AlignBottom(height2);
            rect.height -= height;
            return result;
        }
        /// <summary>
        /// 把一个Rect的宽度调整成一个新的指定值
        /// </summary>
        /// <param name="rect">原来的rect</param>
        /// <param name="width">指定宽度</param>
        /// <returns>一个新的指定宽度的rect</returns>
        public static Rect SetWidth(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect with the specified height.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   height:
        //     The desired height of the new Rect.
        public static Rect SetHeight(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect with the specified size.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   width:
        //     The desired width of the new Rect.
        //
        //   height:
        //     The desired height of the new Rect.
        public static Rect SetSize(this Rect rect, float width, float height)
        {
            rect.width = width;
            rect.height = height;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect with the specified size.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   widthAndHeight:
        //     The desired width and height of the new Rect.
        public static Rect SetSize(this Rect rect, float widthAndHeight)
        {
            rect.width = widthAndHeight;
            rect.height = widthAndHeight;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect with the specified size.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   size:
        //     The desired size of the new Rect.
        public static Rect SetSize(this Rect rect, Vector2 size)
        {
            rect.size = size;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect that has been inserted by the specified amount on the X-axis.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   padding:
        //     The desired padding.
        public static Rect HorizontalPadding(this Rect rect, float padding)
        {
            rect.x += padding;
            rect.width -= padding * 2f;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect that has been inserted by the specified amount on the X-axis.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   left:
        //     Desired padding on the left side.
        //
        //   right:
        //     Desired padding on the right side.
        public static Rect HorizontalPadding(this Rect rect, float left, float right)
        {
            rect.x += left;
            rect.width -= left + right;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect that has been inserted by the specified amount on the Y-axis.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   padding:
        //     The desired padding.
        public static Rect VerticalPadding(this Rect rect, float padding)
        {
            rect.y += padding;
            rect.height -= padding * 2f;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect that has been inserted by the specified amount on the Y-axis.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   top:
        //     The desired padding on the top.
        //
        //   bottom:
        //     The desired padding on the bottom.
        public static Rect VerticalPadding(this Rect rect, float top, float bottom)
        {
            rect.y += top;
            rect.height -= top + bottom;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect that has been inserted by the specified amount.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   padding:
        //     The desired padding.
        public static Rect Padding(this Rect rect, float padding)
        {
            rect.position += new Vector2(padding, padding);
            rect.size -= new Vector2(padding, padding) * 2f;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect that has been inserted by the specified amount.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   horizontal:
        //     The desired horizontal padding.
        //
        //   vertical:
        //     The desired vertical padding.
        public static Rect Padding(this Rect rect, float horizontal, float vertical)
        {
            rect.position += new Vector2(horizontal, vertical);
            rect.size -= new Vector2(horizontal, vertical) * 2f;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect that has been inserted by the specified amount.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   left:
        //     The desired padding on the left.
        //
        //   right:
        //     The desired padding on the right.
        //
        //   top:
        //     The desired padding on the top.
        //
        //   bottom:
        //     The desired padding on the bottom.
        public static Rect Padding(this Rect rect, float left, float right, float top, float bottom)
        {
            rect.position += new Vector2(left, top);
            rect.size -= new Vector2(left + right, top + bottom);
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified width, that has been aligned to the left of
        //     the original Rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   width:
        //     The desired width of the new Rect.
        public static Rect AlignLeft(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified width, that has been aligned to horizontal
        //     center of the original Rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   width:
        //     The desired width of the new Rect.
        public static Rect AlignCenter(this Rect rect, float width)
        {
            rect.x = rect.x + rect.width * 0.5f - width * 0.5f;
            rect.width = width;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified width and height in the center of the provided
        //     rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   width:
        //     The desired width of the new Rect.
        //
        //   height:
        //     The desired height of the new Rect.
        public static Rect AlignCenter(this Rect rect, float width, float height)
        {
            rect.x = rect.x + rect.width * 0.5f - width * 0.5f;
            rect.y = rect.y + rect.height * 0.5f - height * 0.5f;
            rect.width = width;
            rect.height = height;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified width, that has been aligned to the right
        //     of the original Rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   width:
        //     The desired width of the new Rect.
        public static Rect AlignRight(this Rect rect, float width)
        {
            rect.x = rect.x + rect.width - width;
            rect.width = width;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified width, that has been aligned to the right
        //     of the original Rect.
        public static Rect AlignRight(this Rect rect, float width, bool clamp)
        {
            if (clamp)
            {
                rect.xMin = Mathf.Max(rect.xMax - width, rect.xMin);
                return rect;
            }

            rect.x = rect.x + rect.width - width;
            rect.width = width;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified height, that has been aligned to the top of
        //     the original Rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   height:
        //     The desired height of the new Rect.
        public static Rect AlignTop(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified height, that has been aligned to the vertical
        //     middle of the original Rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   height:
        //     The desired height of the new Rect.
        public static Rect AlignMiddle(this Rect rect, float height)
        {
            rect.y = rect.y + rect.height * 0.5f - height * 0.5f;
            rect.height = height;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified height, that has been aligned to the bottom
        //     of the original Rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   height:
        //     The desired height of the new Rect.
        public static Rect AlignBottom(this Rect rect, float height)
        {
            rect.y = rect.y + rect.height - height;
            rect.height = height;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified width, that has been aligned horizontally
        //     to the center of the original rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   width:
        //     The desired width of the new Rect.
        public static Rect AlignCenterX(this Rect rect, float width)
        {
            rect.x = rect.x + rect.width * 0.5f - width * 0.5f;
            rect.width = width;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified height, that has been aligned vertically to
        //     the center of the original rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   height:
        //     The desired height of the new Rect.
        public static Rect AlignCenterY(this Rect rect, float height)
        {
            rect.y = rect.y + rect.height * 0.5f - height * 0.5f;
            rect.height = height;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified width and height, that has been aligned horizontally
        //     and vertically to the center of the original rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   size:
        //     The desired width and height of the new Rect.
        public static Rect AlignCenterXY(this Rect rect, float size)
        {
            rect.y = rect.y + rect.height * 0.5f - size * 0.5f;
            rect.x = rect.x + rect.width * 0.5f - size * 0.5f;
            rect.height = size;
            rect.width = size;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect, with the specified width and height, that has been aligned horizontally
        //     and vertically to the center of the original rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   width:
        //     The desired width of the new Rect.
        //
        //   height:
        //     The desired height of the new Rect.
        public static Rect AlignCenterXY(this Rect rect, float width, float height)
        {
            rect.y = rect.y + rect.height * 0.5f - height * 0.5f;
            rect.x = rect.x + rect.width * 0.5f - width * 0.5f;
            rect.width = width;
            rect.height = height;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect that has been expanded by the specified amount.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   expand:
        //     The desired expansion.
        public static Rect Expand(this Rect rect, float expand)
        {
            rect.x -= expand;
            rect.y -= expand;
            rect.height += expand * 2f;
            rect.width += expand * 2f;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect that has been expanded by the specified amount.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   horizontal:
        //     The desired expansion on the X-axis.
        //
        //   vertical:
        //     The desired expansion on the Y-axis.
        public static Rect Expand(this Rect rect, float horizontal, float vertical)
        {
            rect.position -= new Vector2(horizontal, vertical);
            rect.size += new Vector2(horizontal, vertical) * 2f;
            return rect;
        }

        //
        // 摘要:
        //     Returns a Rect that has been expanded by the specified amount.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   left:
        //     The desired expansion on the left.
        //
        //   right:
        //     The desired expansion on the right.
        //
        //   top:
        //     The desired expansion on the top.
        //
        //   bottom:
        //     The desired expansion on the bottom.
        public static Rect Expand(this Rect rect, float left, float right, float top, float bottom)
        {
            rect.position -= new Vector2(left, top);
            rect.size += new Vector2(left + right, top + bottom);
            return rect;
        }

        //
        // 摘要:
        //     Splits a Rect horizontally into the specified number of sub-rects, and returns
        //     a sub-rect for the specified index.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   index:
        //     The index for the subrect. Includes 0, and excludes count.
        //
        //   count:
        //     The amount of subrects the Rect should be split into.
        public static Rect Split(this Rect rect, int index, int count)
        {
            int num = (int)rect.width;
            int num2 = num / count;
            int num3 = num - num2 * count;
            float num4 = rect.x + (float)(num2 * index);
            if (index < num3)
            {
                num4 += (float)index;
                num2++;
            }
            else
            {
                num4 += (float)num3;
            }

            rect.x = num4;
            rect.width = num2;
            return rect;
        }

        //
        // 摘要:
        //     Splits a Rect vertically into the specified number of sub-rects, and returns
        //     a sub-rect for the specified index.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   index:
        //     The index for the subrect. Includes 0, and excludes count.
        //
        //   count:
        //     The amount of subrects the Rect should be split into.
        public static Rect SplitVertical(this Rect rect, int index, int count)
        {
            float num = (rect.height /= count);
            rect.y += num * (float)index;
            return rect;
        }

        //
        // 摘要:
        //     Splits a Rect into a grid from left to right and then down.
        //
        // 参数:
        //   rect:
        //     The original rect.
        //
        //   width:
        //     The width of a grid cell.
        //
        //   height:
        //     The height of a grid cell.
        //
        //   index:
        //     The index of the grid cell.
        public static Rect SplitGrid(this Rect rect, float width, float height, int index)
        {
            int num = (int)(rect.width / width);
            num = ((num <= 0) ? 1 : num);
            int num2 = index % num;
            int num3 = index / num;
            rect.x += (float)num2 * width;
            rect.y += (float)num3 * height;
            rect.width = width;
            rect.height = height;
            return rect;
        }

        //
        // 摘要:
        //     Splits a Rect into a grid from left to right and then down.
        public static Rect SplitTableGrid(this Rect rect, int columnCount, float rowHeight, int index)
        {
            int num = index % columnCount;
            int num2 = index / columnCount;
            float num3 = rect.width / (float)columnCount;
            rect.x += (float)num * num3;
            rect.y += (float)num2 * rowHeight;
            rect.width = num3;
            rect.height = rowHeight;
            return rect;
        }

        //
        // 摘要:
        //     Moves a Rect to the specified center X position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   x:
        //     The desired center x position.
        public static Rect SetCenterX(this Rect rect, float x)
        {
            rect.center = new Vector2(x, rect.center.y);
            return rect;
        }

        //
        // 摘要:
        //     Moves a Rect to the specified center Y position.
        //
        // 参数:
        //   rect:
        //     The desired original Rect.
        //
        //   y:
        //     The desired desired center y position.
        public static Rect SetCenterY(this Rect rect, float y)
        {
            rect.center = new Vector2(rect.center.x, y);
            return rect;
        }

        //
        // 摘要:
        //     Moves a Rect to the specified center position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   x:
        //     The desired center X position.
        //
        //   y:
        //     The desired center Y position.
        public static Rect SetCenter(this Rect rect, float x, float y)
        {
            rect.center = new Vector2(x, y);
            return rect;
        }

        //
        // 摘要:
        //     Moves a Rect to the specified center position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   center:
        //     The desired center position.
        public static Rect SetCenter(this Rect rect, Vector2 center)
        {
            rect.center = center;
            return rect;
        }

        //
        // 摘要:
        //     Moves a Rect to the specified position.
        //
        // 参数:
        //   rect:
        //     The orignal Rect.
        //
        //   position:
        //     The desired position.
        public static Rect SetPosition(this Rect rect, Vector2 position)
        {
            rect.position = position;
            return rect;
        }

        //
        // 摘要:
        //     Resets a Rect's position to zero.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        public static Rect ResetPosition(this Rect rect)
        {
            rect.position = Vector2.zero;
            return rect;
        }

        //
        // 摘要:
        //     Moves a Rect's position by the specified amount.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   move:
        //     The change in position.
        public static Rect AddPosition(this Rect rect, Vector2 move)
        {
            rect.position += move;
            return rect;
        }

        //
        // 摘要:
        //     Moves a Rect's position by the specified amount.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   x:
        //     The x.
        //
        //   y:
        //     The y.
        public static Rect AddPosition(this Rect rect, float x, float y)
        {
            rect.x += x;
            rect.y += y;
            return rect;
        }

        //
        // 摘要:
        //     Sets a Rect's X position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   x:
        //     The desired X position.
        public static Rect SetX(this Rect rect, float x)
        {
            rect.x = x;
            return rect;
        }

        //
        // 摘要:
        //     Adds to a Rect's X position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   x:
        //     The value to add.
        public static Rect AddX(this Rect rect, float x)
        {
            rect.x += x;
            return rect;
        }

        //
        // 摘要:
        //     Subtracts from a Rect's X position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   x:
        //     The value to subtract.
        public static Rect SubX(this Rect rect, float x)
        {
            rect.x -= x;
            return rect;
        }

        //
        // 摘要:
        //     Sets a Rect's Y position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   y:
        //     The desired Y position.
        public static Rect SetY(this Rect rect, float y)
        {
            rect.y = y;
            return rect;
        }

        //
        // 摘要:
        //     Adds to a Rect's Y position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   y:
        //     The value to add.
        public static Rect AddY(this Rect rect, float y)
        {
            rect.y += y;
            return rect;
        }

        //
        // 摘要:
        //     Subtracts a Rect's Y position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   y:
        //     The value to subtract.
        public static Rect SubY(this Rect rect, float y)
        {
            rect.y -= y;
            return rect;
        }

        //
        // 摘要:
        //     Sets the min position of a Rect.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   min:
        //     The desired min position.
        public static Rect SetMin(this Rect rect, Vector2 min)
        {
            rect.min = min;
            return rect;
        }

        //
        // 摘要:
        //     Adds to a Rect's min position.
        //
        // 参数:
        //   rect:
        //     The original rect.
        //
        //   value:
        //     The value to add.
        public static Rect AddMin(this Rect rect, Vector2 value)
        {
            rect.min += value;
            return rect;
        }

        //
        // 摘要:
        //     Subtracts a Rect's min position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   value:
        //     The vlaue to subtract.
        public static Rect SubMin(this Rect rect, Vector2 value)
        {
            rect.min -= value;
            return rect;
        }

        //
        // 摘要:
        //     Sets a Rect's max position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   max:
        //     The desired max position.
        public static Rect SetMax(this Rect rect, Vector2 max)
        {
            rect.max = max;
            return rect;
        }

        //
        // 摘要:
        //     Adds to a Rect's max position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   value:
        //     The value to add.
        public static Rect AddMax(this Rect rect, Vector2 value)
        {
            rect.max += value;
            return rect;
        }

        //
        // 摘要:
        //     Subtracts a Rect's max position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   value:
        //     The value to add.
        public static Rect SubMax(this Rect rect, Vector2 value)
        {
            rect.max -= value;
            return rect;
        }

        //
        // 摘要:
        //     Sets a Rect's X min position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   xMin:
        //     The desired min X position.
        public static Rect SetXMin(this Rect rect, float xMin)
        {
            rect.xMin = xMin;
            return rect;
        }

        //
        // 摘要:
        //     Adds to a Rect's X min position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   value:
        //     The value to add.
        public static Rect AddXMin(this Rect rect, float value)
        {
            rect.xMin += value;
            return rect;
        }

        //
        // 摘要:
        //     Subtracts from a Rect's X min position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   value:
        //     The value to subtract.
        public static Rect SubXMin(this Rect rect, float value)
        {
            rect.xMin -= value;
            return rect;
        }

        //
        // 摘要:
        //     Sets a Rect's X max position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   xMax:
        //     The desired X max position.
        public static Rect SetXMax(this Rect rect, float xMax)
        {
            rect.xMax = xMax;
            return rect;
        }

        //
        // 摘要:
        //     Adds to a Rect's X max position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   value:
        //     The value to add.
        public static Rect AddXMax(this Rect rect, float value)
        {
            rect.xMax += value;
            return rect;
        }

        //
        // 摘要:
        //     Subtracts a Rect's X max position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   value:
        //     The value to subtract.
        public static Rect SubXMax(this Rect rect, float value)
        {
            rect.xMax -= value;
            return rect;
        }

        //
        // 摘要:
        //     Sets a Rect's Y min position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   yMin:
        //     The desired Y min.
        public static Rect SetYMin(this Rect rect, float yMin)
        {
            rect.yMin = yMin;
            return rect;
        }

        //
        // 摘要:
        //     Adds to a Rect's Y min position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   value:
        //     The value to add.
        public static Rect AddYMin(this Rect rect, float value)
        {
            rect.yMin += value;
            return rect;
        }

        //
        // 摘要:
        //     Subtracts a Rect's Y min position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   value:
        //     The value to subtract.
        public static Rect SubYMin(this Rect rect, float value)
        {
            rect.yMin -= value;
            return rect;
        }

        //
        // 摘要:
        //     Sets a Rect's Y max position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   yMax:
        //     The desired Y max position.
        public static Rect SetYMax(this Rect rect, float yMax)
        {
            rect.yMax = yMax;
            return rect;
        }

        //
        // 摘要:
        //     Adds to a Rect's Y max position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   value:
        //     The value to add.
        public static Rect AddYMax(this Rect rect, float value)
        {
            rect.yMax += value;
            return rect;
        }

        //
        // 摘要:
        //     Subtracts from a Rect's Y max position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   value:
        //     The value to subtract.
        public static Rect SubYMax(this Rect rect, float value)
        {
            rect.yMax -= value;
            return rect;
        }

        //
        // 摘要:
        //     Sets a Rect's width, if it is less than the specified value.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   minWidth:
        //     The desired min width.
        public static Rect MinWidth(this Rect rect, float minWidth)
        {
            rect.width = Mathf.Max(rect.width, minWidth);
            return rect;
        }

        //
        // 摘要:
        //     Sets a Rect's width, if it is greater than the specified value.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   maxWidth:
        //     The desired max width.
        public static Rect MaxWidth(this Rect rect, float maxWidth)
        {
            rect.width = Mathf.Min(rect.width, maxWidth);
            return rect;
        }

        //
        // 摘要:
        //     Sets a Rect's height, if it is less than the specified value.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   minHeight:
        //     The desired min height.
        public static Rect MinHeight(this Rect rect, float minHeight)
        {
            rect.height = Mathf.Max(rect.height, minHeight);
            return rect;
        }

        //
        // 摘要:
        //     Sets a Rect's height, if it is greater than the specified value.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   maxHeight:
        //     The desired max height.
        public static Rect MaxHeight(this Rect rect, float maxHeight)
        {
            rect.height = Mathf.Min(rect.height, maxHeight);
            return rect;
        }

        //
        // 摘要:
        //     Expands a rect to contain a given position.
        //
        // 参数:
        //   rect:
        //     The original Rect.
        //
        //   pos:
        //     The position to expand the rect towards.
        public static Rect ExpandTo(this Rect rect, Vector2 pos)
        {
            if (pos.x < rect.xMin)
            {
                rect.xMin = pos.x;
            }
            else if (pos.x > rect.xMax)
            {
                rect.xMax = pos.x;
            }

            if (pos.y < rect.yMin)
            {
                rect.yMin = pos.y;
            }
            else if (pos.y > rect.yMax)
            {
                rect.yMax = pos.y;
            }

            return rect;
        }
    }
}

