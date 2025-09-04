using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;

namespace OpenAI.Responses;

// CUSTOM:
// - Added Experimental attribute.
// - Renamed.
[CodeGenType("ComputerAction")]
[CodeGenVisibility(nameof(Kind), CodeGenVisibility.Public)]
public partial class ComputerCallAction
{
    // CUSTOM: Exposed click action properties.
    public Point? ClickCoordinates => this switch
    {
        InternalComputerActionClick click => new Point(click.X, click.Y),
        _ => null
    };
    public ComputerCallActionMouseButton? ClickMouseButton => (this as InternalComputerActionClick)?.Button;

    // CUSTOM: Exposed double-click action properties.
    public Point? DoubleClickCoordinates => this switch
    {
        InternalComputerActionDoubleClick doubleClick => new Point(doubleClick.X, doubleClick.Y),
        _ => null
    };

    // CUSTOM: Exposed drag action properties.
    public IList<Point> DragPath => (this as InternalComputerActionDrag)?.Path.Select(item => new Point(item.X, item.Y)).ToList();

    // CUSTOM: Exposed key press action properties.
    public IList<string> KeyPressKeyCodes => (this as InternalComputerActionKeyPress)?.Keys;

    // CUSTOM: Exposed move action properties.
    public Point? MoveCoordinates => this switch
    {
        InternalComputerActionMove move => new Point(move.X, move.Y),
        _ => null
    };

    // CUSTOM: Exposed scroll action properties.
    public Point? ScrollCoordinates => this switch
    {
        InternalComputerActionScroll scroll => new Point(scroll.X, scroll.Y),
        _ => null
    };
    public int? ScrollHorizontalOffset => (this as InternalComputerActionScroll)?.ScrollX;
    public int? ScrollVerticalOffset => (this as InternalComputerActionScroll)?.ScrollY;

    // CUSTOM: Exposed type action properties.
    public string TypeText => (this as InternalComputerActionTypeKeys)?.Text;

    public static ComputerCallAction CreateClickAction(Point clickCoordinates, ComputerCallActionMouseButton clickMouseButton)
    {
        return new InternalComputerActionClick(
            kind: ComputerCallActionKind.Click,
            additionalBinaryDataProperties: null,
            button: clickMouseButton,
            x: clickCoordinates.X,
            y: clickCoordinates.Y);
    }

    public static ComputerCallAction CreateDoubleClickAction(Point doubleClickCoordinates, ComputerCallActionMouseButton doubleClickMouseButton)
    {
        return new InternalComputerActionDoubleClick(
            kind: ComputerCallActionKind.DoubleClick,
            additionalBinaryDataProperties: null,
            x: doubleClickCoordinates.X,
            y: doubleClickCoordinates.Y);
    }

    public static ComputerCallAction CreateDragAction(IList<Point> dragPath)
    {
        return new InternalComputerActionDrag(
            kind: ComputerCallActionKind.Drag,
            additionalBinaryDataProperties: null,
            path: dragPath.Select(item => new InternalCoordinate(item.X, item.Y)).ToList());
    }

    public static ComputerCallAction CreateKeyPressAction(IList<string> keyCodes)
    {
        return new InternalComputerActionKeyPress(
            kind: ComputerCallActionKind.KeyPress,
            additionalBinaryDataProperties: null,
            keys: keyCodes);
    }

    public static ComputerCallAction CreateMoveAction(Point moveCoordinates)
    {
        return new InternalComputerActionMove(
            kind: ComputerCallActionKind.Move,
            additionalBinaryDataProperties: null,
            x: moveCoordinates.X,
            y: moveCoordinates.Y);
    }

    public static ComputerCallAction CreateScreenshotAction()
    {
        return new InternalComputerActionScreenshot(
            kind: ComputerCallActionKind.Screenshot,
            additionalBinaryDataProperties: null);
    }

    public static ComputerCallAction CreateScrollAction(Point scrollCoordinates, int horizontalOffset, int verticalOffset)
    {
        return new InternalComputerActionScroll(
            kind: ComputerCallActionKind.Scroll,
            additionalBinaryDataProperties: null,
            x: scrollCoordinates.X,
            y: scrollCoordinates.Y,
            scrollX: horizontalOffset,
            scrollY: verticalOffset);
    }

    public static ComputerCallAction CreateTypeAction(string typeText)
    {
        return new InternalComputerActionTypeKeys(
            kind: ComputerCallActionKind.Type,
            additionalBinaryDataProperties: null,
            text: typeText);
    }

    public static ComputerCallAction CreateWaitAction()
    {
        return new InternalComputerActionWait(
            kind: ComputerCallActionKind.Wait,
            additionalBinaryDataProperties: null);
    }
}