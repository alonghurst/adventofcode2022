namespace AdventOfCode2022Cs.Day12;

/// <summary>
/// A collection of objects which are added with a priority. Objects can only be retrieved from the IndexedPriorityQueue in the order of priority
/// </summary>
/// <typeparam name="T"></typeparam>
public class IndexedPriorityQueue<T> where T : class
{
    // A list containing the priorities of the items
    private readonly List<float> _priorities = new();
    // A list containing the items in the queue
    private readonly List<T> _items = new();

    // A list of indexes used to resort the queue
    private readonly List<int> _tempIndexes = new();

    /// <summary>
    /// Inserts an item into the IPQ with a given priority. 
    /// </summary>
    /// <param name="item">The item to insert</param>
    /// <param name="priority">The priority of the item in the queue</param>
    public void Insert(T item, float priority, bool sort = true)
    {
        // Add the new item
        _items.Add(item);
        // Add the priority of the item
        _priorities.Add(priority);
            
        if(sort)
            Sort();
    }

    /// <summary>
    /// Clears the IPQ, removing all items and priorities.
    /// </summary>
    public void Clear()
    {
        // Clear all lists
        _tempIndexes.Clear();
        _priorities.Clear();
        _items.Clear();
    }

    /// <summary>
    /// Sorts the IPQ to ensure all items are in the order of priority
    /// </summary>
    public void Sort()
    {
        // Reset the temporary indexes
        _tempIndexes.Clear();

        int numItems = _items.Count;
        int num = 1;

        _tempIndexes.Add(0);

        // Sort the items, putting the index of the item into _tempIndexes
        while (_tempIndexes.Count < numItems)
        {
            int newCount = _tempIndexes.Count();

            for (int i = 0; i < newCount; i++)
            {
                if (_priorities[num] < _priorities[_tempIndexes[i]])
                {
                    _tempIndexes.Insert(i, num);
                    break;
                }
                else if (i == _tempIndexes.Count - 1)
                {
                    _tempIndexes.Add(num);
                }
            }
            num++;
        }

        // Temporary holders for the items and priorities
        float[] tempPriorities = new float[numItems];
        T[] tempItems = new T[numItems];

        // Put the values and items into the temporary arrays
        for (int i = 0; i < numItems; i++)
        {
            tempPriorities[i] = _priorities[_tempIndexes[i]];
            tempItems[i] = _items[_tempIndexes[i]];
        }

        // Reset the actual items and priorities list
        _items.Clear();
        _priorities.Clear();

        // Refill the items and priorities from the temp arrays
        for (int i = 0; i < numItems; i++)
        {
            _items.Add(tempItems[i]);
            _priorities.Add(tempPriorities[i]);
        }
    }

    /// <summary>
    /// Removes the next item from the IPQ. 
    /// </summary>
    /// <returns>The item with the highest priority.</returns>
    public T Pop()
    {
        T toReturn = _items[0];
        _items.RemoveAt(0);
        _priorities.RemoveAt(0);

        return toReturn;
    }

    /// <summary>
    /// Returns true if the IPQ is empty 
    /// </summary>
    /// <returns></returns>
    public bool Empty()
    {
        return _items.Count == 0;
    }

    /// <summary>
    /// Changes the priority of an item in the IPQ.
    /// Causes a Sort operation
    /// </summary>
    /// <param name="toChange">The item to change</param>
    /// <param name="newPriority">The new priority to set.</param>
    public void ChangePriority(T toChange, float newPriority)
    {
        int count = _items.Count;

        // Find the item's index, change the priority and resort.
        for (int i = 0; i < count; i++)
        {
            if (_items[i] == toChange)
            {
                _priorities[i] = newPriority;
                Sort();
                return;
            }
        }
    }

    //public void Output()
    //{
    //    for (int i = 0; i < _items.Count; i++)
    //    {
    //        Abstractions.Logger.Info(_items[i].ToString() + " " + _priorities[i].ToString());
    //    }
    //}

    //public void OutputStatus()
    //{
    //    Abstractions.Logger.Info(_items.Count + " Values");
    //}
}