using UnityEngine;
using Object = UnityEngine.Object;

namespace Uncooked {

    internal class CookIngredientAction : ISequentialAction {

        private readonly PlayerControls player;

        private int state;
        private Action currentAction;

        private readonly ClientCookableContainer cookableContainer;
        private ClientCookingStation cookingStation;

        public CookIngredientAction(PlayerControls player) {
            Logger.Log("CookIngredientAction instantiated");
            
            this.player = player;
            state = 0;

            cookableContainer =
                ComponentUtil.GetClosestMatchingComponent<ClientCookableContainer>(player.transform.position,
                    IsCookablePot);
            Logger.Log($"Cookable container pos: {Logger.FormatPosition(cookableContainer.transform.position)}");
            bool cookableOnStation = false;
            foreach (var possibleCookingStation in Object.FindObjectsOfType<ClientCookingStation>()) {
                Logger.Log($"Station pos: {Logger.FormatPosition(possibleCookingStation.transform.position)}");
                if (IsCookableOnStation(cookableContainer, possibleCookingStation)) {
                    cookableOnStation = true;
                    cookingStation = possibleCookingStation;
                    break;
                }
            }

            if (cookableOnStation) {
                currentAction =
                    new PathFindAction(
                        player,
                        cookingStation
                    );

                state = 3;
            } else {
                currentAction =
                    new PathFindAction(
                        player,
                        cookableContainer
                    );
            }
        }

        public bool Update() {
            switch (state) {
                case 0:
                    if (currentAction.Update()) {
                        currentAction.End();
                        state = 1;
                        currentAction = new PickDropAction(player);
                    }

                    return false;
                case 1:
                    if (currentAction.Update()) {
                        currentAction.End();
                        
                        cookingStation = ComponentUtil.GetClosestComponent<ClientCookingStation>(player.transform.position);

                        if (IsCookableOnStation(cookableContainer, cookingStation)) {
                            state = 5;
                        } else {
                            state = 2;

                            currentAction = new PickDropAction(player);
                        }
                    }
                    
                    return false;
                case 2:
                    if (currentAction.Update()) {
                        currentAction.End();
                        state = 3;
                        
                        currentAction =
                            new PathFindAction(
                                player,
                                cookingStation
                            );
                    }

                    return false;
                case 3:
                    if (currentAction.Update()) {
                        currentAction.End();
                        state = 4;
                        
                        currentAction = new PickDropAction(player);
                    }

                    return false;
                
                case 4:
                    if (currentAction.Update()) {
                        currentAction.End();

                        state = 5;
                    }

                    return false;
                
                case 5:
                    if (cookableContainer.GetCookingHandler().GetCookingProgress() >=
                        cookableContainer.GetCookingHandler().AccessCookingTime) {

                        state = 6;
                        
                        currentAction =
                            new PathFindAction(
                                player,
                                cookingStation
                            );
                    }
                    
                    return false;
                case 6:
                    if (currentAction.Update()) {
                        currentAction.End();

                        state = 7;
                        
                        currentAction = new PickDropAction(player);
                    }
                    
                    return false;
                case 7:
                    if (currentAction.Update()) {
                        currentAction.End();

                        return true;
                    }

                    return false;
                default:
                    return false;
            }
        }

        public void End() {
            currentAction.End();
        }

        private bool IsCookableOnStation(ClientCookableContainer container, ClientCookingStation station) {
            if (ReflectionUtil.GetValue(station, "m_itemPot") == null) {
                Logger.Log("Cookable is not on station (null)");
                return false;
            }
            
            IClientCookable clientCookable = (IClientCookable) ReflectionUtil.GetValue(station, "m_itemPot");
            
            if (clientCookable.Equals(container.GetCookingHandler())) {
                Logger.Log("Cookable is on station");
            } else {
                Logger.Log("Cookable is not on station");
            }

            return clientCookable.Equals(container.GetCookingHandler());
        }

        private bool IsCookablePot(Component component) {
            if (component is ClientCookableContainer clientCookableContainer) {
                return clientCookableContainer.GetCookingHandler().AccessCookingType.m_uID == 20068;
            }

            return false;
        }

        public bool IsIdle() {
            if (state == 5) {
                return cookableContainer.GetCookingHandler().GetCookingProgress() <
                       cookableContainer.GetCookingHandler().AccessCookingTime;
            }

            return false;
        }
    }

}
