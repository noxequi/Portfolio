using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MoonSharp.Interpreter;
using System.IO;

public class BarGenerator : MonoBehaviour
{
    public GameObject barPrefab;
    public Transform barParent;
    public int numberOfBars = 16;
    public float barSpacing = 0f;
    public TMP_Dropdown barCountDropdown;
    public TMP_Dropdown sortOrderDropdown;
    public TMP_Dropdown sortAlgorithmDropdown;
    public GameObject nextStepButton;
    public GameObject previousStepButton;
    public GameObject resetButton;
    public GameObject finalStateButton;
    public GameObject playButton;
    public GameObject pauseButton;
    public GameObject Editor;
    public TMP_Text stepCounterText;
    public TMP_InputField luaInputField;

    public Image StartColorimage;
    public Image EndColorimage;
    public Image BackgroundColorimage;
    public Image background;

    [SerializeField] private Slider stepSlider; 
    private List<float[]> steps = new List<float[]>(); 
    private int currentStep = 0; 
    private float[] barHeights; 
    private float[] initialBarHeights; 
    private bool isPlaying = false; 

    private Color startColor; 
    private Color endColor;
    private Color backgroundColor; 

    void Start()
    {
        barCountDropdown.onValueChanged.AddListener(OnBarCountChanged);
        sortOrderDropdown.onValueChanged.AddListener(OnSortOrderChanged);
        sortAlgorithmDropdown.onValueChanged.AddListener(OnSortAlgorithmChanged);
        nextStepButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(NextStep);
        previousStepButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(PreviousStep);
        resetButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ResetToInitialState);
        finalStateButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ResetToFinalState);
        playButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Play);
        pauseButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(Pause);
        Editor.SetActive(false);
        
        stepSlider.onValueChanged.AddListener(OnSliderValueChanged);
        startColor = StartColorimage.color;
        endColor = EndColorimage.color;
        backgroundColor = BackgroundColorimage.color;

        UpdateStepCounterUI();
        GenerateBars();
    }

    void Update()
    {
        startColor = StartColorimage.color;
        endColor = EndColorimage.color;
        backgroundColor = BackgroundColorimage.color;
        background.color = BackgroundColorimage.color; 

        UpdateStepCounterUI();
    }

    void OnStartColorChanged(Color newColor)
    {
        startColor = newColor;
        StartColorimage.color = newColor; 
        UpdateBars(barHeights);
    }

    void OnEndColorChanged(Color newColor)
    {
        endColor = newColor;
        EndColorimage.color = newColor;
        UpdateBars(barHeights);
    }

    void OnBackgroundColorChanged(Color newColor)
    {
        backgroundColor = newColor;
        BackgroundColorimage.color = newColor;
        background.color = newColor; 
    }

    void ApplyColorGradient(RectTransform rect, float height)
    {
        float normalizedHeight = Mathf.InverseLerp(0f, Mathf.Max(barHeights), height);
        Color barColor = Color.Lerp(startColor, endColor, normalizedHeight);
        rect.GetComponent<Image>().color = barColor;
    }

    void OnBarCountChanged(int value)
    {
        numberOfBars = int.Parse(barCountDropdown.options[value].text);
        ResetBars();
    }

    void OnSortOrderChanged(int value)
    {
        ResetBars();
    }

    void OnSortAlgorithmChanged(int value)
    {
        ResetBars();
    }

    void GenerateBars()
    {
        foreach (Transform child in barParent)
        {
            Destroy(child.gameObject);
        }

        RectTransform parentRect = barParent.GetComponent<RectTransform>();
        float parentHeight = parentRect.rect.height;

        barHeights = new float[numberOfBars];
        for (int i = 0; i < numberOfBars; i++)
        {
            barHeights[i] = (parentHeight * (i + 1)) / numberOfBars; 
        }

        initialBarHeights = (float[])barHeights.Clone();

        int sortOrder = sortOrderDropdown.value;
        SortBars(barHeights, sortOrder);

        for (int i = 0; i < numberOfBars; i++)
        {
            GameObject newBar = Instantiate(barPrefab, barParent);
            RectTransform rect = newBar.GetComponent<RectTransform>();
            float barWidth = parentRect.rect.width / numberOfBars;
            rect.sizeDelta = new Vector2(barWidth, barHeights[i]);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = new Vector2(i * (barWidth + barSpacing), 0);

            ApplyColorGradient(rect, barHeights[i]);
        }

        SortAlgorithm();
    }

    void SortBars(float[] barHeights, int sortOrder)
    {
        switch (sortOrder)
        {
            case 0: //Ascending
                System.Array.Sort(barHeights);
                break;
            case 1: //Descending
                System.Array.Sort(barHeights);
                System.Array.Reverse(barHeights);
                break;
            case 2: //Random
                ShuffleArray(barHeights);
                break;
        }
    }

    void ShuffleArray(float[] array)
    {
        System.Random rand = new System.Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            float temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    void SortAlgorithm()
    {
        steps.Clear();
        currentStep = 0;

        int algorithmOption = sortAlgorithmDropdown.value;
        if (algorithmOption == 0)  // Bubble Sort
        {
            BubbleSort(barHeights);
        }
        else if (algorithmOption == 1)  // Selection Sort
        {
            SelectionSort(barHeights);
        }
        else if (algorithmOption == 2)  // Insertion Sort
        {
            InsertionSort(barHeights);
        }
        else if (algorithmOption == 3)  // Shell Sort
        {
            ShellSort(barHeights);
        }
        else if (algorithmOption == 4)  // Quick Sort
        {
            QuickSort(barHeights, 0, barHeights.Length - 1);
        }
        else if (algorithmOption == 5)  // Heap Sort
        {
            HeapSort(barHeights);
        }
        else if (algorithmOption == 6)  // Merge Sort
        {
            MergeSort(barHeights, 0, barHeights.Length - 1);
        }
        else if (algorithmOption == 7)  // Bin Sort
        {
            BinSort(barHeights);
        }
        else if (algorithmOption == 8)  // Your Sort
        {
            Editor.SetActive(true);
            YourSort(barHeights);
        }

        stepSlider.minValue = 0;
        stepSlider.maxValue = steps.Count - 1;
        stepSlider.value = currentStep;

        if (steps.Count > 0)
        {
            DrawBars(steps[0]);
        }
    }

    // Bubble Sort
    void BubbleSort(float[] array)
    {
        float[] tempArray = (float[])array.Clone();
        steps.Add((float[])tempArray.Clone());

        for (int i = 0; i < tempArray.Length - 1; i++)
        {
            for (int j = 0; j < tempArray.Length - i - 1; j++)
            {
                if (tempArray[j] > tempArray[j + 1])
                {
                    float temp = tempArray[j];
                    tempArray[j] = tempArray[j + 1];
                    tempArray[j + 1] = temp;
                    steps.Add((float[])tempArray.Clone());
                }
            }
        }
    }

    // Selection Sort
    void SelectionSort(float[] array)
    {
        float[] tempArray = (float[])array.Clone();
        steps.Add((float[])tempArray.Clone());

        for (int i = 0; i < tempArray.Length - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < tempArray.Length; j++)
            {
                if (tempArray[j] < tempArray[minIndex])
                {
                    minIndex = j;
                    steps.Add((float[])tempArray.Clone());
                }
            }
            if (minIndex != i)
            {
                float temp = tempArray[i];
                tempArray[i] = tempArray[minIndex];
                tempArray[minIndex] = temp;
                steps.Add((float[])tempArray.Clone());
            }
        }
    }

    // Insertion Sort
    void InsertionSort(float[] array)
    {
        float[] tempArray = (float[])array.Clone();
        steps.Add((float[])tempArray.Clone());

        for (int i = 1; i < tempArray.Length; i++)
        {
            float key = tempArray[i];
            int j = i - 1;
            while (j >= 0 && tempArray[j] > key)
            {
                tempArray[j + 1] = tempArray[j];
                j--;
                steps.Add((float[])tempArray.Clone());
            }
            tempArray[j + 1] = key;
            steps.Add((float[])tempArray.Clone());
        }
    }

    // Shell Sort
    void ShellSort(float[] array)
    {
        float[] tempArray = (float[])array.Clone();
        steps.Add((float[])tempArray.Clone());

        int n = tempArray.Length;
        int gap = n / 2;
        while (gap > 0)
        {
            for (int i = gap; i < n; i++)
            {
                float temp = tempArray[i];
                int j = i;
                while (j >= gap && tempArray[j - gap] > temp)
                {
                    tempArray[j] = tempArray[j - gap];
                    j -= gap;
                    steps.Add((float[])tempArray.Clone());
                }
                tempArray[j] = temp;
                steps.Add((float[])tempArray.Clone());
            }
            gap /= 2;
        }
    }

    // Quick Sort
    void QuickSort(float[] array, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(array, low, high);

            QuickSort(array, low, pivotIndex);
            QuickSort(array, pivotIndex + 1, high);
        }
    }

    int Partition(float[] array, int low, int high)
    {
        float pivot = array[(low + high) / 2]; 
        int left = low - 1;
        int right = high + 1;

        while (true)
        {
            do
            {
                left++;
            } while (array[left] < pivot);

            do
            {
                right--;
            } while (array[right] > pivot);

            if (left >= right)
            {
                return right;
            }

            float temp = array[left];
            array[left] = array[right];
            array[right] = temp;

            steps.Add((float[])array.Clone());
        }
    }


    // Heap Sort
    void HeapSort(float[] array)
    {
        float[] tempArray = (float[])array.Clone();
        steps.Add((float[])tempArray.Clone());

        int n = tempArray.Length;
        for (int i = n / 2 - 1; i >= 0; i--)
        {
            Heapify(tempArray, n, i);
            steps.Add((float[])tempArray.Clone());
        }
        for (int i = n - 1; i > 0; i--)
        {
            float temp = tempArray[0];
            tempArray[0] = tempArray[i];
            tempArray[i] = temp;
            steps.Add((float[])tempArray.Clone());
            Heapify(tempArray, i, 0);
            steps.Add((float[])tempArray.Clone());
        }
    }

    void Heapify(float[] array, int n, int i)
    {
        int largest = i;
        int left = 2 * i + 1;
        int right = 2 * i + 2;
        if (left < n && array[left] > array[largest])
            largest = left;
        if (right < n && array[right] > array[largest])
            largest = right;
        if (largest != i)
        {
            float swap = array[i];
            array[i] = array[largest];
            array[largest] = swap;
            Heapify(array, n, largest);
        }
    }

    // Merge Sort
    void MergeSort(float[] array, int left, int right)
    {
        if (left < right)
        {
            int mid = left + (right - left) / 2;
            MergeSort(array, left, mid);
            MergeSort(array, mid + 1, right);
            Merge(array, left, mid, right);
        }
    }

    void Merge(float[] array, int left, int mid, int right)
    {
        int n1 = mid - left + 1;
        int n2 = right - mid;
        float[] leftArray = new float[n1];
        float[] rightArray = new float[n2];

        System.Array.Copy(array, left, leftArray, 0, n1);
        System.Array.Copy(array, mid + 1, rightArray, 0, n2);

        int i = 0, j = 0, k = left;
        while (i < n1 && j < n2)
        {
            if (leftArray[i] <= rightArray[j])
            {
                array[k] = leftArray[i];
                i++;
            }
            else
            {
                array[k] = rightArray[j];
                j++;
            }
            k++;
            steps.Add((float[])array.Clone());
        }
        while (i < n1)
        {
            array[k] = leftArray[i];
            i++;
            k++;
        }
        while (j < n2)
        {
            array[k] = rightArray[j];
            j++;
            k++;
        }
        steps.Add((float[])array.Clone());
    }

    // Bin Sort
    void BinSort(float[] array)
    {
        float[] tempArray = (float[])array.Clone();

        int n = tempArray.Length;
        if (n == 0)
            return;

        float minValue = tempArray[0];
        float maxValue = tempArray[0];
        for (int i = 1; i < n; i++)
        {
            if (tempArray[i] < minValue)
                minValue = tempArray[i];
            if (tempArray[i] > maxValue)
                maxValue = tempArray[i];
        }

        int binCount = n;
        List<List<float>> bins = new List<List<float>>(binCount);
        for (int i = 0; i < binCount; i++)
        {
            bins.Add(new List<float>());
        }

        for (int i = 0; i < n; i++)
        {
            int index = (int)((tempArray[i] - minValue) / (maxValue - minValue) * (binCount - 1));
            bins[index].Add(tempArray[i]);
        }

        int currentIndex = 0;
        foreach (var bin in bins)
        {
            bin.Sort();
            foreach (var value in bin)
            {
                tempArray[currentIndex] = value;
                currentIndex++;
            }
            steps.Add((float[])tempArray.Clone());
        }
    }

    void YourSort(float[] array)
    {
        string luaFilePath = Path.Combine(Application.streamingAssetsPath, "LuaScripts/myScript.lua");

        string luaCode;
        try
        {
            luaCode = File.ReadAllText(luaFilePath);
        }
        catch (IOException ex)
        {
            //Debug.LogError("Luaスクリプトの読み込みに失敗: " + ex.Message);
            return;
        }

        Script script = new Script();
        try
        {
            script.DoString(luaCode);

            DynValue luaFunction = script.Globals.Get("sortBars");

            if (luaFunction.Type == DataType.Function)
            {
                Table luaTable = new Table(script);
                for (int i = 0; i < array.Length; i++)
                {
                    luaTable.Set(i + 1, DynValue.NewNumber(array[i]));
                }

                DynValue result = luaFunction.Function.Call(luaTable);

                if (result.Type == DataType.Table)
                {
                    Table stepsTable = result.Table;
                    steps.Clear();

                    foreach (var step in stepsTable.Values)
                    {
                        if (step.Type == DataType.Table)
                        {
                            Table stepTable = step.Table;
                            float[] stepArray = new float[array.Length];
                            for (int i = 0; i < array.Length; i++)
                            {
                                stepArray[i] = (float)stepTable.Get(i + 1).Number;
                            }
                            steps.Add(stepArray);
                        }
                    }

                    float[] finalStep = steps[^1];
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = finalStep[i];
                    }
                    UpdateBars(array);
                }
                else
                {
                    //Debug.LogError("Lua関数 'sortBars' の戻り値が不正: ");
                }
            }
            else
            {
                //Debug.LogError("Lua関数 'sortBars' が見つかりません: ");
            }
        }
        catch (ScriptRuntimeException ex)
        {
            //Debug.LogError("Luaスクリプトの実行に失敗: " + ex.DecoratedMessage);
        }
    }

    public void onClick_CloseLua()
    {
        Editor.SetActive(false);
    }

    public void NextStep()
    {
        if (currentStep < steps.Count - 1)
        {
            currentStep++;
            stepSlider.value = currentStep; 
            UpdateBars(steps[currentStep]);
            UpdateStepCounterUI();
        }
    }

    public void PreviousStep()
    {
        if (currentStep > 0)
        {
            currentStep--;
            stepSlider.value = currentStep; 
            UpdateBars(steps[currentStep]);
            UpdateStepCounterUI();
        }
    }


    void UpdateBars(float[] heights)
    {
        RectTransform parentRect = barParent.GetComponent<RectTransform>();
        float parentWidth = parentRect.rect.width;

        int i = 0;
        foreach (Transform child in barParent)
        {
            RectTransform rect = child.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(parentWidth / numberOfBars, heights[i]);

            ApplyColorGradient(rect, heights[i]);

            i++;
        }
    }

    public void ResetToInitialState()
    {
        UpdateBars(initialBarHeights);
        currentStep = 0;
        UpdateStepCounterUI();
    }

    public void ResetToFinalState()
    {
        if (steps.Count > 0)
        {
            UpdateBars(steps[steps.Count - 1]);
            currentStep = steps.Count; 
            UpdateStepCounterUI(); 
        }
    }

    public void Play()
    {
        isPlaying = true;
        StartCoroutine(PlaySteps());
    }

    public void Pause()
    {
        isPlaying = false;
    }

    void UpdateStepCounterUI()
    {
        if (stepCounterText != null)
        {
            stepCounterText.text = $"Step {currentStep}/{steps.Count}"; 
        }
    }

    void OnSliderValueChanged(float value)
    {
        int stepIndex = Mathf.RoundToInt(value);
        if (stepIndex != currentStep && stepIndex >= 0 && stepIndex < steps.Count)
        {
            currentStep = stepIndex;
            UpdateBars(steps[currentStep]);
            UpdateStepCounterUI();   
        }
    }

    void DrawBars(float[] heights)
    {
        foreach (Transform child in barParent)
        {
            Destroy(child.gameObject);
        }

        RectTransform parentRect = barParent.GetComponent<RectTransform>();
        float parentWidth = parentRect.rect.width;

        for (int i = 0; i < heights.Length; i++)
        {
            GameObject newBar = Instantiate(barPrefab, barParent);
            RectTransform rect = newBar.GetComponent<RectTransform>();
            float barWidth = parentWidth / heights.Length;
            rect.sizeDelta = new Vector2(barWidth, heights[i]);
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(0, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = new Vector2(i * (barWidth + barSpacing), 0);

            ApplyColorGradient(rect, heights[i]);
        }
    }


    private IEnumerator PlaySteps()
    {
        while (isPlaying && currentStep < steps.Count)
        {
            float[] stepData = steps[currentStep];
            UpdateBars(stepData);
            currentStep++;
            yield return new WaitForSeconds(0.01f); 
        }
    }

    void ResetBars()
    {
        GenerateBars();
    }
}
