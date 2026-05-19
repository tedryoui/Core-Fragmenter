using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace _.Scripts.User_Interface
{
    public class WorldUtilityWindowViewModel : AbstractUserInterfaceViewModel
    {
        public enum UtilityBit
        {
            Health,
            Loading,
            Timer,
            Button
        }

        private VisualElement _container;
        private VisualElement _healthBit;
        private VisualElement _loadingBit;
        private VisualElement _timerBit;
        private VisualElement _buttonBit;
        private Label         _healthLabel;
        private Label         _healthValue;
        private VisualElement _healthFill;
        private Label         _loadingLabel;
        private Label         _loadingValue;
        private VisualElement _loadingFill;
        private Label         _timerTitle;
        private Label         _timerValue;
        private VisualElement _timerFill;
        private Button        _actionButton;

        public event Action ActionButtonClicked;

        protected override void SoftInitialize() { }

        private void Start()
        {
            var root = Document.rootVisualElement;

            _container    = root.Q<VisualElement>("Container");
            _healthBit    = root.Q<VisualElement>("health-bit");
            _loadingBit   = root.Q<VisualElement>("loading-bit");
            _timerBit     = root.Q<VisualElement>("timer-bit");
            _buttonBit    = root.Q<VisualElement>("button-bit");
            _healthLabel  = root.Q<Label>("health-label");
            _healthValue  = root.Q<Label>("health-value");
            _healthFill   = root.Q<VisualElement>("health-fill");
            _loadingLabel = root.Q<Label>("loading-label");
            _loadingValue = root.Q<Label>("loading-value");
            _loadingFill  = root.Q<VisualElement>("loading-fill");
            _timerTitle   = root.Q<Label>("timer-title");
            _timerValue   = root.Q<Label>("timer-value");
            _timerFill    = root.Q<VisualElement>("timer-fill");
            _actionButton = root.Q<Button>("action-button");

            _actionButton.clicked += OnActionButtonClicked;
            
            SetVisible(false);
        }

        private void OnDestroy()
        {
            if (_actionButton != null)
                _actionButton.clicked -= OnActionButtonClicked;
        }

        public void SetVisible(bool isVisible)
        {
            if (_container == null)
                return;

            _container.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public bool IsVisible()
        {
            if (_container == null)
                return false;
            
            return _container.style.display == DisplayStyle.Flex;
        }

        public void Show() => SetVisible(true);

        public void Hide() => SetVisible(false);

        public void SetBitVisible(UtilityBit bit, bool isVisible)
        {
            var element = GetBitElement(bit);
            if (element == null)
                return;

            element.EnableInClassList("world-utility-window__bit--hidden", !isVisible);
        }

        public bool IsBitEnabled(UtilityBit bit) => !IsBitDisabled(bit);

        public bool IsBitDisabled(UtilityBit bit)
        {
            var element = GetBitElement(bit);
            if (element == null)
                return true;

            return element.ClassListContains("world-utility-window__bit--hidden");
        }

        public void SetHealth(int current, int max, string label = null)
        {
            max = Mathf.Max(1, max);
            current = Mathf.Clamp(current, 0, max);

            if (!string.IsNullOrEmpty(label))
                _healthLabel.text = label;

            _healthValue.text = $"{current} / {max}";
            SetProgressFill(_healthFill, (float)current / max);
        }

        public void SetHealthLabel(string label)
        {
            if (!string.IsNullOrEmpty(label))
                _healthLabel.text = label;
        }

        public void SetLoadingProgress(float normalizedProgress, string label = null)
        {
            normalizedProgress = Mathf.Clamp01(normalizedProgress);

            if (!string.IsNullOrEmpty(label))
                _loadingLabel.text = label;

            _loadingValue.text = $"{Mathf.RoundToInt(normalizedProgress * 100f)}%";
            SetProgressFill(_loadingFill, normalizedProgress);
        }

        public void SetLoadingLabel(string label)
        {
            if (!string.IsNullOrEmpty(label))
                _loadingLabel.text = label;
        }

        public void SetTimer(float remainingSeconds, float totalSeconds, string title = null)
        {
            totalSeconds = Mathf.Max(0.001f, totalSeconds);
            remainingSeconds = Mathf.Clamp(remainingSeconds, 0f, totalSeconds);

            if (!string.IsNullOrEmpty(title))
                _timerTitle.text = title;

            _timerValue.text = FormatTimer(remainingSeconds);
            SetProgressFill(_timerFill, remainingSeconds / totalSeconds);
        }

        public void SetTimerTitle(string title)
        {
            if (!string.IsNullOrEmpty(title))
                _timerTitle.text = title;
        }

        public void ConfigureButton(string text, bool isInteractable = true)
        {
            if (!string.IsNullOrEmpty(text))
                _actionButton.text = text;

            _actionButton.SetEnabled(isInteractable);
        }

        public void SetButtonInteractable(bool isInteractable) => _actionButton.SetEnabled(isInteractable);

        private void OnActionButtonClicked() => ActionButtonClicked?.Invoke();

        private VisualElement GetBitElement(UtilityBit bit) => bit switch
        {
            UtilityBit.Health  => _healthBit,
            UtilityBit.Loading => _loadingBit,
            UtilityBit.Timer   => _timerBit,
            UtilityBit.Button  => _buttonBit,
            _                  => null
        };

        private static string FormatTimer(float seconds)
        {
            var total = Mathf.CeilToInt(Mathf.Max(0f, seconds));
            var minutes = total / 60;
            var secs = total % 60;
            return minutes > 0 ? $"{minutes}:{secs:D2}" : $"{secs}s";
        }

        private static void SetProgressFill(VisualElement fill, float normalizedProgress)
        {
            if (fill == null)
                return;

            fill.style.width = new Length(Mathf.Clamp01(normalizedProgress) * 100f, LengthUnit.Percent);
        }
    }
}
