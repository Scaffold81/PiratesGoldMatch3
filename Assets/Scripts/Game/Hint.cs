using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game.Gameplay
{
    public class Hint
    {
        private float _timeToHint = 1;
        private bool _hintTimerActive;
        private AvalableNodeForMatch _currentHint;
        private List<AvalableNodeForMatch> _avalableNodeForMatches = new List<AvalableNodeForMatch>();

        public Hint(float timeToHint)
        {
            _timeToHint = timeToHint;
        }

        public void StartHintTimer(List<AvalableNodeForMatch> avalableNodeForMatches)
        {
            if (_hintTimerActive) return;
            _hintTimerActive = true;
            _avalableNodeForMatches = avalableNodeForMatches;
            HintTimer();
        }

        private async void HintTimer()
        {
            await Task.Delay(TimeSpan.FromSeconds(_timeToHint));
            _hintTimerActive = false;
            SetHint();
        }

        private void SetHint()
        {
            if (_currentHint.FirstNode != null)
            {
                _currentHint.FirstNode.HightlightOff();
                _currentHint.SecondNode.HightlightOff();
            }

            var randomValue = UnityEngine.Random.Range(0, _avalableNodeForMatches.Count);

            _currentHint = _avalableNodeForMatches[randomValue];
            _currentHint.FirstNode.HightlightOn();
            _currentHint.SecondNode.HightlightOn();
        }

        public void OffHint()
        {
            if (_currentHint.FirstNode == null || _currentHint.SecondNode==null) return;
            _currentHint.FirstNode.HightlightOff();
            _currentHint.SecondNode.HightlightOff();
            _currentHint.FirstNode = null;
            _currentHint.SecondNode = null;
        }

    }
}

