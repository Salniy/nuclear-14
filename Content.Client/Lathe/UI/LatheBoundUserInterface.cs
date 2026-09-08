using System.Collections.Generic;
using Content.Shared.DeltaV.Salvage; // DeltaV
using Content.Shared.Lathe;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client.Lathe.UI
{
    [UsedImplicitly]
    public sealed class LatheBoundUserInterface : BoundUserInterface
    {
        [ViewVariables]
        private LatheMenu? _menu;
        public LatheBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
        }

        protected override void Open()
        {
            base.Open();

            _menu = this.CreateWindow<LatheMenu>();
            _menu.SetEntity(Owner);
            _menu.OpenCenteredRight();

            _menu.OnServerListButtonPressed += _ =>
            {
                SendMessage(new ConsoleServerSelectionMessage());
            };

            _menu.RecipeQueueAction += (recipe, amount) =>
            {
                SendMessage(new LatheQueueRecipeMessage(recipe, amount));
            };

            _menu.OnClaimMiningPoints += () => SendMessage(new LatheClaimMiningPointsMessage()); // DeltaV
        }

        protected override void UpdateState(BoundUserInterfaceState state)
        {
            base.UpdateState(state);

            switch (state)
            {
                case LatheUpdateState msg:
                    if (_menu != null)
                        _menu.Recipes = msg.Recipes;
                    _menu?.PopulateRecipes();
                    _menu?.UpdateCategories();

                    var queue = new List<LatheRecipePrototype>();
                    if (msg.CurrentlyProducing != null)
                        queue.Add(msg.CurrentlyProducing);
                    queue.AddRange(msg.Queue);

                    _menu?.PopulateQueueList(queue);
                    break;
            }
        }
    }
}
