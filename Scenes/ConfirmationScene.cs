namespace UnfathomableParking.Scenes;

/// <summary>
/// A scene that displays a confirmation prompt.
/// </summary>
public class ConfirmationScene : EnumSelectionScene<ConfirmationSceneOptions>
{
    /// <summary>
    /// Creates a new confirmation scene.
    /// </summary>
    /// <param name="initialSelectedIndex">The initial index of the selected option. Defaults to 0 (Yes).</param>
    /// <param name="onConfirm">The action to invoke when the user confirms the selection.</param>
    /// <param name="onCancel">The action to invoke when the user cancels the selection.</param>
    /// <param name="title">The title of the confirmation scene.</param>
    public ConfirmationScene(
        uint initialSelectedIndex = 0,
        Action? onConfirm = null,
        Action? onCancel = null,
        string? title = null
    ) : base(initialSelectedIndex, title: title, onSelect: delegate(ConfirmationSceneOptions selection)
    {
        switch (selection)
        {
            case ConfirmationSceneOptions.Yes:
                onConfirm?.Invoke();
                break;
            case ConfirmationSceneOptions.No:
                onCancel?.Invoke();
                break;
        }
    })
    {
    }
}

/// <summary>
/// The options for the confirmation scene.
/// </summary>
public enum ConfirmationSceneOptions
{
    /// <summary>
    /// The affirmative answer option.
    /// </summary>
    Yes,

    /// <summary>
    /// The negative answer option.
    /// </summary>
    No,
}
