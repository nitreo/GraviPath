using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Invert.StateMachine;


public class ShipStateMachine : ShipStateMachineBase {
    
    public ShipStateMachine(ViewModel vm, string propertyName) : 
            base(vm, propertyName) {
    }
}
