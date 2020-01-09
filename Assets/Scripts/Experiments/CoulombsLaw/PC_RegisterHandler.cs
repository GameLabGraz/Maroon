using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_RegisterHandler : MonoBehaviour
{
    private List<PC_RegisterBase> _registers = new List<PC_RegisterBase>();
    private PC_RegisterBase _selectedRegister;
    
    public void RegisterBaseRegister(PC_RegisterBase register, bool selectedOnStart)
    {
        _registers.Add(register);
        if (selectedOnStart)
        {
            _selectedRegister = register;
            _selectedRegister.SetActive();
        }
        else
        {
            register.SetInactive();
        }
    }

    public void SelectRegister(PC_RegisterBase registerBase)
    {
        if (_selectedRegister == registerBase) return;
        _selectedRegister.SetInactive();
        _selectedRegister = registerBase;
        _selectedRegister.SetActive();
    }
}
