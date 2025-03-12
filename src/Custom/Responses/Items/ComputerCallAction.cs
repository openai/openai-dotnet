using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace OpenAI.Responses;

[CodeGenType("ResponsesComputerCallItemAction")]
public partial class ComputerCallAction
{
    // CUSTOM:
    // - Renamed.
    // - Made public.
    // - Removed setter.
    [CodeGenMember("Type")]
    public ComputerCallActionKind Kind { get; }

    // CUSTOM: Exposed click action properties.
    public Point? ClickCoordinates => this switch
    {
        InternalResponsesComputerCallClickAction click => new Point(click.X, click.Y),
        _ => null
    };
    public ComputerCallActionMouseButton? ClickMouseButton => (this as InternalResponsesComputerCallClickAction)?.Button;

    // CUSTOM: Exposed double-click action properties.
    public Point? DoubleClickCoordinates => this switch
    {
        InternalResponsesComputerCallDoubleClickAction doubleClick => new Point(doubleClick.X, doubleClick.Y),
        _ => null
    };

    // CUSTOM: Exposed drag action properties.
    public IList<Point> DragPath => (this as InternalResponsesComputerCallDragAction)?.Path.Select(item => new Point(item.X, item.Y)).ToList();

    // CUSTOM: Exposed key press action properties.
    public IList<string> KeyPressKeyCodes => (this as InternalResponsesComputerCallKeyPressAction)?.Keys;

    // CUSTOM: Exposed move action properties.
    public Point? MoveCoordinates => this switch
    {
        InternalResponsesComputerCallMoveAction move => new Point(move.X, move.Y),
        _ => null
    };

    // CUSTOM: Exposed scroll action properties.
    public Point? ScrollCoordinates => this switch
    {
        InternalResponsesComputerCallScrollAction scroll => new Point(scroll.X, scroll.Y),
        _ => null
    };
    public int? ScrollHorizontalOffset => (this as InternalResponsesComputerCallScrollAction)?.ScrollX;
    public int? ScrollVerticalOffset => (this as InternalResponsesComputerCallScrollAction)?.ScrollY;

    // CUSTOM: Exposed type action properties.
    public string TypeText => (this as InternalResponsesComputerCallTypeAction)?.Text;

    public static ComputerCallAction CreateClickAction(Point clickCoordinates, ComputerCallActionMouseButton clickMouseButton)
    {
        return new InternalResponsesComputerCallClickAction(
            kind: ComputerCallActionKind.Click,
            additionalBinaryDataProperties: null,
            button: clickMouseButton,
            x: clickCoordinates.X,
            y: clickCoordinates.Y);
    }

    public static ComputerCallAction CreateDoubleClickAction(Point doubleClickCoordinates, ComputerCallActionMouseButton doubleClickMouseButton)
    {
        return new InternalResponsesComputerCallDoubleClickAction(
            kind: ComputerCallActionKind.DoubleClick,
            additionalBinaryDataProperties: null,
            x: doubleClickCoordinates.X,
            y: doubleClickCoordinates.Y);
    }

    public static ComputerCallAction CreateDragAction(IList<Point> dragPath)
    {
        return new InternalResponsesComputerCallDragAction(
            kind: ComputerCallActionKind.Drag,
            additionalBinaryDataProperties: null,
            path: dragPath.Select(item => new InternalResponsesComputerCallDragActionPath(item.X, item.Y)).ToList());
    }

    public static ComputerCallAction CreateKeyPressAction(IList<string> keyCodes)
    {
        return new InternalResponsesComputerCallKeyPressAction(
            kind: ComputerCallActionKind.KeyPress,
            additionalBinaryDataProperties: null,
            keys: keyCodes);
    }

    public static ComputerCallAction CreateMoveAction(Point moveCoordinates)
    {
        return new InternalResponsesComputerCallMoveAction(
            kind: ComputerCallActionKind.Move,
            additionalBinaryDataProperties: null,
            x: moveCoordinates.X,
            y: moveCoordinates.Y);
    }

    public static ComputerCallAction CreateScreenshotAction()
    {
        return new InternalResponsesComputerCallScreenshotAction(
            kind: ComputerCallActionKind.Screenshot,
            additionalBinaryDataProperties: null);
    }

    public static ComputerCallAction CreateScrollAction(Point scrollCoordinates, int horizontalOffset, int verticalOffset)
    {
        return new InternalResponsesComputerCallScrollAction(
            kind: ComputerCallActionKind.Scroll,
            additionalBinaryDataProperties: null,
            x: scrollCoordinates.X,
            y: scrollCoordinates.Y,
            scrollX: horizontalOffset,
            scrollY: verticalOffset);
    }

    public static ComputerCallAction CreateTypeAction(string typeText)
    {
        return new InternalResponsesComputerCallTypeAction(
            kind: ComputerCallActionKind.Type,
            additionalBinaryDataProperties: null,
            text: typeText);
    }

    public static ComputerCallAction CreateWaitAction()
    {
        return new InternalResponsesComputerCallWaitAction(
            kind: ComputerCallActionKind.Wait,
            additionalBinaryDataProperties: null);
    }
}