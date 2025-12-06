using System;
using System.IO;
using WpfExplorer.Support.Local.Models;

public class NavigatorService
{
    private Stack<Memento> backStack = new Stack<Memento>();
    private Stack<Memento> forwardStack = new Stack<Memento>();
    public FileInfoBase currentLocation;

    public FileInfoBase Currnet => currentLocation;
    public event EventHandler<LocationChangedEventArgs> LocationChanged;

    public void ChangeLocation(FileInfoBase newLocation)
    {
        if (currentLocation != null)
        {
            SaveState();
        }

        currentLocation = newLocation;
        OnLocationChanged(new LocationChangedEventArgs(currentLocation));
    }
    public void GoBack()
    {
        if (backStack.Any())
        {
            RestoreState(backStack, forwardStack);
        }
    }


    public void GoForward()
    {
        if (forwardStack.Any())
        {
            RestoreState(forwardStack, backStack);
        }
    }
    public void GoToParent()
    {
        DirectoryInfo parentDir = Directory.GetParent(currentLocation.FullPath);
        if (parentDir != null)
        {
            ChangeLocation(new FileInfoBase { FullPath = parentDir.FullName });
        }
    }
    private void SaveState()
    {
        backStack.Push(new Memento(currentLocation.FullPath));
    }
    private void RestoreState(Stack<Memento> popFrom, Stack<Memento> pushTo)
    {
        pushTo.Push(new Memento(currentLocation.FullPath));
        currentLocation.FullPath = popFrom.Pop().GetSavedFullPath();
        OnLocationChanged(new LocationChangedEventArgs(currentLocation));
    }
    protected virtual void OnLocationChanged(LocationChangedEventArgs e)
    {
        LocationChanged?.Invoke(this, e);
    }


}
