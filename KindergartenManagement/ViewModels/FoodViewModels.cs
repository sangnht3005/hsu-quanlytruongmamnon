using System.Collections.ObjectModel;
using System.Windows.Input;
using KindergartenManagement.BLO;
using KindergartenManagement.DTO;

namespace KindergartenManagement.ViewModels;

// ======================== Daily Menu Management ========================
public class DailyMenuManagementViewModel : ViewModelBase
{
    private readonly IFoodManagementBlo _blo;
    
    private ObservableCollection<Menu> _menus = new();
    private ObservableCollection<Dish> _dishes = new();
    private ObservableCollection<MenuDish> _menuDishes = new();
    private Menu? _selectedMenu;
    private Dish? _selectedDishToAdd;
    private MenuDish? _selectedMenuDish;
    private string _name = string.Empty;
    private int _dayOfWeek = 1; // Default Monday
    private string _mealType = "Breakfast";
    private string? _description;

    public ObservableCollection<Menu> Menus
    {
        get => _menus;
        set => SetProperty(ref _menus, value);
    }

    public ObservableCollection<Dish> Dishes
    {
        get => _dishes;
        set => SetProperty(ref _dishes, value);
    }

    public ObservableCollection<MenuDish> MenuDishes
    {
        get => _menuDishes;
        set => SetProperty(ref _menuDishes, value);
    }

    public Menu? SelectedMenu
    {
        get => _selectedMenu;
        set
        {
            if (SetProperty(ref _selectedMenu, value))
            {
                if (_selectedMenu != null)
                {
                    Name = _selectedMenu.Name;
                    DayOfWeek = _selectedMenu.DayOfWeek;
                    MealType = _selectedMenu.MealType;
                    Description = _selectedMenu.Description;
                    _ = LoadMenuDishesAsync();
                }
                else
                {
                    ClearForm();
                }
            }
        }
    }

    public Dish? SelectedDishToAdd
    {
        get => _selectedDishToAdd;
        set => SetProperty(ref _selectedDishToAdd, value);
    }

    public MenuDish? SelectedMenuDish
    {
        get => _selectedMenuDish;
        set => SetProperty(ref _selectedMenuDish, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public int DayOfWeek
    {
        get => _dayOfWeek;
        set => SetProperty(ref _dayOfWeek, value);
    }

    public string MealType
    {
        get => _mealType;
        set => SetProperty(ref _mealType, value);
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand SaveMenuCommand { get; }
    public ICommand DeleteMenuCommand { get; }
    public ICommand AddNewCommand { get; }
    public ICommand AddDishCommand { get; }
    public ICommand RemoveDishCommand { get; }
    public ICommand GenerateMealTicketsCommand { get; }
    public ICommand SaveAutoSettingsCommand { get; }

    // Auto-generate meal settings (app-level)
    private bool _autoBreakfast;
    private bool _autoLunch;
    private bool _autoSnack;
    private bool _autoDinner;
    public bool AutoBreakfast { get => _autoBreakfast; set => SetProperty(ref _autoBreakfast, value); }
    public bool AutoLunch { get => _autoLunch; set => SetProperty(ref _autoLunch, value); }
    public bool AutoSnack { get => _autoSnack; set => SetProperty(ref _autoSnack, value); }
    public bool AutoDinner { get => _autoDinner; set => SetProperty(ref _autoDinner, value); }

    // Date for generating meal tickets
    private DateTime _generationDate = DateTime.Today;
    public DateTime GenerationDate { get => _generationDate; set => SetProperty(ref _generationDate, value); }

    public DailyMenuManagementViewModel(IFoodManagementBlo blo)
    {
        _blo = blo;
        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        SaveMenuCommand = new RelayCommand(async _ => await SaveMenuAsync());
        DeleteMenuCommand = new RelayCommand(async _ => await DeleteMenuAsync(), _ => SelectedMenu != null);
        AddNewCommand = new RelayCommand(_ => ClearForm());
        AddDishCommand = new RelayCommand(async _ => await AddDishAsync(), _ => SelectedMenu != null && SelectedDishToAdd != null);
        RemoveDishCommand = new RelayCommand(async _ => await RemoveDishAsync(), _ => SelectedMenuDish != null);
        GenerateMealTicketsCommand = new RelayCommand(async _ => await GenerateMealTicketsAsync(), _ => SelectedMenu != null);
        SaveAutoSettingsCommand = new RelayCommand(_ => SaveAutoSettings());
        
        LoadAutoSettings();
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var menus = await _blo.GetAllMenusAsync();
        Menus = new ObservableCollection<Menu>(menus);

        var dishes = await _blo.GetAllDishesAsync();
        Dishes = new ObservableCollection<Dish>(dishes.Where(d => d.IsActive).ToList());
    }

    private async Task LoadMenuDishesAsync()
    {
        if (SelectedMenu != null)
        {
            var menuDishes = await _blo.GetMenuDishesAsync(SelectedMenu.Id);
            MenuDishes = new ObservableCollection<MenuDish>(menuDishes);
        }
    }

    private async Task SaveMenuAsync()
    {
        var menu = SelectedMenu ?? new Menu();
        menu.Name = Name;
        menu.DayOfWeek = DayOfWeek;
        menu.MealType = MealType;
        menu.Description = Description;

        await _blo.SaveMenuAsync(menu);
        await LoadDataAsync();
        
        if (SelectedMenu == null)
            ClearForm();
    }

    private async Task DeleteMenuAsync()
    {
        if (SelectedMenu != null)
        {
            await _blo.DeleteMenuAsync(SelectedMenu.Id);
            await LoadDataAsync();
            ClearForm();
        }
    }

    private async Task AddDishAsync()
    {
        if (SelectedMenu != null && SelectedDishToAdd != null)
        {
            await _blo.AddMenuDishAsync(SelectedMenu.Id, SelectedDishToAdd.Id);
            await LoadMenuDishesAsync();
            SelectedDishToAdd = null;
        }
    }

    private async Task RemoveDishAsync()
    {
        if (SelectedMenu != null && SelectedMenuDish != null)
        {
            await _blo.RemoveMenuDishAsync(SelectedMenu.Id, SelectedMenuDish.DishId);
            await LoadMenuDishesAsync();
        }
    }

    private async Task GenerateMealTicketsAsync()
    {
        if (SelectedMenu != null)
        {
            // Tạo phiếu ăn cho ngày được chọn
            var count = await _blo.GenerateMealTicketsForMenuAsync(SelectedMenu.Id, GenerationDate);
            // Could show a message: Generated {count} meal tickets
        }
    }

    private void LoadAutoSettings()
    {
        var s = KindergartenManagement.Utilities.AutoMealSettings.Load();
        AutoBreakfast = s.Breakfast;
        AutoLunch = s.Lunch;
        AutoSnack = s.Snack;
        AutoDinner = s.Dinner;
    }

    private void SaveAutoSettings()
    {
        var s = new KindergartenManagement.Utilities.AutoMealSettings
        {
            Breakfast = AutoBreakfast,
            Lunch = AutoLunch,
            Snack = AutoSnack,
            Dinner = AutoDinner
        };
        s.Save();
    }

    private void ClearForm()
    {
        SelectedMenu = null;
        Name = string.Empty;
        DayOfWeek = 1; // Monday
        MealType = "Breakfast";
        Description = null;
        MenuDishes.Clear();
        SelectedDishToAdd = null;
    }
}

// ======================== MealTicket Management ========================
public class MealTicketManagementViewModel : ViewModelBase
{
    private readonly IFoodManagementBlo _blo;
    
    private ObservableCollection<MealTicket> _mealTickets = new();
    private ObservableCollection<MealTicket> _filteredMealTickets = new();
    private MealTicket? _selectedMealTicket;
    private DateTime _filterDate = DateTime.Today;
    private bool _isConsumed;
    private string? _notes;

    public ObservableCollection<MealTicket> MealTickets
    {
        get => _mealTickets;
        set => SetProperty(ref _mealTickets, value);
    }

    public ObservableCollection<MealTicket> FilteredMealTickets
    {
        get => _filteredMealTickets;
        set => SetProperty(ref _filteredMealTickets, value);
    }

    public MealTicket? SelectedMealTicket
    {
        get => _selectedMealTicket;
        set
        {
            if (SetProperty(ref _selectedMealTicket, value))
            {
                if (_selectedMealTicket != null)
                {
                    IsConsumed = _selectedMealTicket.IsConsumed;
                    Notes = _selectedMealTicket.Notes;
                }
                else
                {
                    ClearForm();
                }
            }
        }
    }

    public DateTime FilterDate
    {
        get => _filterDate;
        set => SetProperty(ref _filterDate, value);
    }

    public bool IsConsumed
    {
        get => _isConsumed;
        set => SetProperty(ref _isConsumed, value);
    }

    public string? Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand ApplyFilterCommand { get; }
    public ICommand SaveMealTicketCommand { get; }
    public ICommand DeleteMealTicketCommand { get; }

    public MealTicketManagementViewModel(IFoodManagementBlo blo)
    {
        _blo = blo;
        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        ApplyFilterCommand = new RelayCommand(async _ => await ApplyFilterAsync());
        SaveMealTicketCommand = new RelayCommand(async _ => await SaveMealTicketAsync(), _ => SelectedMealTicket != null);
        DeleteMealTicketCommand = new RelayCommand(async _ => await DeleteMealTicketAsync(), _ => SelectedMealTicket != null);
        
        _ = ApplyFilterAsync();
    }

    private async Task LoadDataAsync()
    {
        var mealTickets = await _blo.GetMealTicketsByDateAsync(FilterDate);
        MealTickets = new ObservableCollection<MealTicket>(mealTickets);
        FilteredMealTickets = new ObservableCollection<MealTicket>(mealTickets);
    }

    private async Task ApplyFilterAsync()
    {
        var mealTickets = await _blo.GetMealTicketsByDateAsync(FilterDate);
        FilteredMealTickets = new ObservableCollection<MealTicket>(mealTickets);
    }

    private async Task SaveMealTicketAsync()
    {
        if (SelectedMealTicket != null)
        {
            SelectedMealTicket.IsConsumed = IsConsumed;
            SelectedMealTicket.Notes = Notes;

            await _blo.SaveMealTicketAsync(SelectedMealTicket);
            await ApplyFilterAsync();
        }
    }

    private async Task DeleteMealTicketAsync()
    {
        if (SelectedMealTicket != null)
        {
            await _blo.DeleteMealTicketAsync(SelectedMealTicket.Id);
            await ApplyFilterAsync();
            ClearForm();
        }
    }

    private void ClearForm()
    {
        SelectedMealTicket = null;
        IsConsumed = false;
        Notes = null;
    }
}



// ======================== Supplier Management ========================
public class SupplierManagementViewModel : ViewModelBase
{
    private readonly IFoodManagementBlo _blo;
    
    private ObservableCollection<Supplier> _suppliers = new();
    private Supplier? _selectedSupplier;
    private string _name = string.Empty;
    private string? _contactPerson;
    private string? _phone;
    private string? _email;
    private string? _address;
    private string? _notes;
    private bool _isActive = true;

    public ObservableCollection<Supplier> Suppliers
    {
        get => _suppliers;
        set => SetProperty(ref _suppliers, value);
    }

    public Supplier? SelectedSupplier
    {
        get => _selectedSupplier;
        set
        {
            if (SetProperty(ref _selectedSupplier, value))
            {
                if (_selectedSupplier != null)
                {
                    Name = _selectedSupplier.Name;
                    ContactPerson = _selectedSupplier.ContactPerson;
                    Phone = _selectedSupplier.Phone;
                    Email = _selectedSupplier.Email;
                    Address = _selectedSupplier.Address;
                    Notes = _selectedSupplier.Notes;
                    IsActive = _selectedSupplier.IsActive;
                }
                else
                {
                    ClearForm();
                }
            }
        }
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string? ContactPerson
    {
        get => _contactPerson;
        set => SetProperty(ref _contactPerson, value);
    }

    public string? Phone
    {
        get => _phone;
        set => SetProperty(ref _phone, value);
    }

    public string? Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string? Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    public string? Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand SaveSupplierCommand { get; }
    public ICommand DeleteSupplierCommand { get; }
    public ICommand AddNewCommand { get; }

    public SupplierManagementViewModel(IFoodManagementBlo blo)
    {
        _blo = blo;
        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        SaveSupplierCommand = new RelayCommand(async _ => await SaveSupplierAsync());
        DeleteSupplierCommand = new RelayCommand(async _ => await DeleteSupplierAsync(), _ => SelectedSupplier != null);
        AddNewCommand = new RelayCommand(_ => ClearForm());
        
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var suppliers = await _blo.GetAllSuppliersAsync();
        Suppliers = new ObservableCollection<Supplier>(suppliers);
    }

    private async Task SaveSupplierAsync()
    {
        var supplier = SelectedSupplier ?? new Supplier();
        supplier.Name = Name;
        supplier.ContactPerson = ContactPerson;
        supplier.Phone = Phone;
        supplier.Email = Email;
        supplier.Address = Address;
        supplier.Notes = Notes;
        supplier.IsActive = IsActive;

        await _blo.SaveSupplierAsync(supplier);
        await LoadDataAsync();
        ClearForm();
    }

    private async Task DeleteSupplierAsync()
    {
        if (SelectedSupplier != null)
        {
            await _blo.DeleteSupplierAsync(SelectedSupplier.Id);
            await LoadDataAsync();
            ClearForm();
        }
    }

    private void ClearForm()
    {
        SelectedSupplier = null;
        Name = string.Empty;
        ContactPerson = null;
        Phone = null;
        Email = null;
        Address = null;
        Notes = null;
        IsActive = true;
    }
}

// ======================== Ingredient Management ========================
public class IngredientManagementViewModel : ViewModelBase
{
    private readonly IFoodManagementBlo _blo;
    
    private ObservableCollection<Ingredient> _ingredients = new();
    private ObservableCollection<Supplier> _suppliers = new();
    private Ingredient? _selectedIngredient;
    private string _name = string.Empty;
    private string? _category;
    private string _unit = string.Empty;
    private decimal _caloriesPer100g;
    private decimal _unitPrice;
    private Guid? _supplierId;
    private string? _description;
    private bool _isActive = true;

    public ObservableCollection<Ingredient> Ingredients
    {
        get => _ingredients;
        set => SetProperty(ref _ingredients, value);
    }

    public ObservableCollection<Supplier> Suppliers
    {
        get => _suppliers;
        set => SetProperty(ref _suppliers, value);
    }

    public Ingredient? SelectedIngredient
    {
        get => _selectedIngredient;
        set
        {
            if (SetProperty(ref _selectedIngredient, value))
            {
                if (_selectedIngredient != null)
                {
                    Name = _selectedIngredient.Name;
                    Category = _selectedIngredient.Category;
                    Unit = _selectedIngredient.Unit;
                    CaloriesPer100g = _selectedIngredient.CaloriesPer100g;
                    UnitPrice = _selectedIngredient.UnitPrice;
                    SupplierId = _selectedIngredient.SupplierId;
                    Description = _selectedIngredient.Description;
                    IsActive = _selectedIngredient.IsActive;
                }
                else
                {
                    ClearForm();
                }
            }
        }
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string? Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    public string Unit
    {
        get => _unit;
        set => SetProperty(ref _unit, value);
    }

    public decimal CaloriesPer100g
    {
        get => _caloriesPer100g;
        set => SetProperty(ref _caloriesPer100g, value);
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set => SetProperty(ref _unitPrice, value);
    }

    public Guid? SupplierId
    {
        get => _supplierId;
        set => SetProperty(ref _supplierId, value);
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand SaveIngredientCommand { get; }
    public ICommand DeleteIngredientCommand { get; }
    public ICommand AddNewCommand { get; }

    public IngredientManagementViewModel(IFoodManagementBlo blo)
    {
        _blo = blo;
        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        SaveIngredientCommand = new RelayCommand(async _ => await SaveIngredientAsync());
        DeleteIngredientCommand = new RelayCommand(async _ => await DeleteIngredientAsync(), _ => SelectedIngredient != null);
        AddNewCommand = new RelayCommand(_ => ClearForm());
        
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var ingredients = await _blo.GetAllIngredientsAsync();
        Ingredients = new ObservableCollection<Ingredient>(ingredients);

        var suppliers = await _blo.GetAllSuppliersAsync();
        Suppliers = new ObservableCollection<Supplier>(suppliers);
    }

    private async Task SaveIngredientAsync()
    {
        var ingredient = SelectedIngredient ?? new Ingredient();
        ingredient.Name = Name;
        ingredient.Category = Category;
        ingredient.Unit = Unit;
        ingredient.CaloriesPer100g = CaloriesPer100g;
        ingredient.UnitPrice = UnitPrice;
        ingredient.SupplierId = SupplierId;
        ingredient.Description = Description;
        ingredient.IsActive = IsActive;

        await _blo.SaveIngredientAsync(ingredient);
        await LoadDataAsync();
        ClearForm();
    }

    private async Task DeleteIngredientAsync()
    {
        if (SelectedIngredient != null)
        {
            await _blo.DeleteIngredientAsync(SelectedIngredient.Id);
            await LoadDataAsync();
            ClearForm();
        }
    }

    private void ClearForm()
    {
        SelectedIngredient = null;
        Name = string.Empty;
        Category = null;
        Unit = string.Empty;
        CaloriesPer100g = 0;
        UnitPrice = 0;
        SupplierId = null;
        Description = null;
        IsActive = true;
    }
}

// ======================== Dish Management ========================
public class DishManagementViewModel : ViewModelBase
{
    private readonly IFoodManagementBlo _blo;
    
    private ObservableCollection<Dish> _dishes = new();
    private ObservableCollection<Ingredient> _ingredients = new();
    private ObservableCollection<DishIngredient> _dishIngredients = new();
    private Dish? _selectedDish;
    private Ingredient? _selectedIngredientToAdd;
    private DishIngredient? _selectedDishIngredient;
    private string _name = string.Empty;
    private string? _category;
    private string? _description;
    private string? _recipe;
    private decimal _totalCalories;
    private decimal _totalCost;
    private decimal _sellingPrice;
    private decimal _ingredientQuantity;
    private bool _isActive = true;

    public ObservableCollection<Dish> Dishes
    {
        get => _dishes;
        set => SetProperty(ref _dishes, value);
    }

    public ObservableCollection<Ingredient> Ingredients
    {
        get => _ingredients;
        set => SetProperty(ref _ingredients, value);
    }

    public ObservableCollection<DishIngredient> DishIngredients
    {
        get => _dishIngredients;
        set => SetProperty(ref _dishIngredients, value);
    }

    public Dish? SelectedDish
    {
        get => _selectedDish;
        set
        {
            if (SetProperty(ref _selectedDish, value))
            {
                if (_selectedDish != null)
                {
                    Name = _selectedDish.Name;
                    Category = _selectedDish.Category;
                    Description = _selectedDish.Description;
                    Recipe = _selectedDish.Recipe;
                    TotalCalories = _selectedDish.TotalCalories;
                    TotalCost = _selectedDish.TotalCost;
                    SellingPrice = _selectedDish.SellingPrice;
                    IsActive = _selectedDish.IsActive;
                    _ = LoadDishIngredientsAsync();
                }
                else
                {
                    ClearForm();
                }
            }
        }
    }

    public Ingredient? SelectedIngredientToAdd
    {
        get => _selectedIngredientToAdd;
        set => SetProperty(ref _selectedIngredientToAdd, value);
    }

    public DishIngredient? SelectedDishIngredient
    {
        get => _selectedDishIngredient;
        set => SetProperty(ref _selectedDishIngredient, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string? Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string? Recipe
    {
        get => _recipe;
        set => SetProperty(ref _recipe, value);
    }

    public decimal TotalCalories
    {
        get => _totalCalories;
        set => SetProperty(ref _totalCalories, value);
    }

    public decimal TotalCost
    {
        get => _totalCost;
        set => SetProperty(ref _totalCost, value);
    }

    public decimal SellingPrice
    {
        get => _sellingPrice;
        set => SetProperty(ref _sellingPrice, value);
    }

    public decimal IngredientQuantity
    {
        get => _ingredientQuantity;
        set => SetProperty(ref _ingredientQuantity, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public ICommand LoadDataCommand { get; }
    public ICommand SaveDishCommand { get; }
    public ICommand DeleteDishCommand { get; }
    public ICommand AddNewCommand { get; }
    public ICommand AddIngredientCommand { get; }
    public ICommand RemoveIngredientCommand { get; }
    public ICommand RecalculateCommand { get; }

    public DishManagementViewModel(IFoodManagementBlo blo)
    {
        _blo = blo;
        LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
        SaveDishCommand = new RelayCommand(async _ => await SaveDishAsync());
        DeleteDishCommand = new RelayCommand(async _ => await DeleteDishAsync(), _ => SelectedDish != null);
        AddNewCommand = new RelayCommand(_ => ClearForm());
        AddIngredientCommand = new RelayCommand(async _ => await AddIngredientAsync(), _ => SelectedDish != null && SelectedIngredientToAdd != null);
        RemoveIngredientCommand = new RelayCommand(async _ => await RemoveIngredientAsync(), _ => SelectedDishIngredient != null);
        RecalculateCommand = new RelayCommand(async _ => await RecalculateAsync(), _ => SelectedDish != null);
        
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var dishes = await _blo.GetAllDishesAsync();
        Dishes = new ObservableCollection<Dish>(dishes);

        var ingredients = await _blo.GetAllIngredientsAsync();
        Ingredients = new ObservableCollection<Ingredient>(ingredients.Where(i => i.IsActive).ToList());
    }

    private async Task LoadDishIngredientsAsync()
    {
        if (SelectedDish != null)
        {
            var dishIngredients = await _blo.GetDishIngredientsAsync(SelectedDish.Id);
            DishIngredients = new ObservableCollection<DishIngredient>(dishIngredients);
        }
    }

    private async Task SaveDishAsync()
    {
        var dish = SelectedDish ?? new Dish();
        dish.Name = Name;
        dish.Category = Category;
        dish.Description = Description;
        dish.Recipe = Recipe;
        dish.SellingPrice = SellingPrice;
        dish.IsActive = IsActive;

        await _blo.SaveDishAsync(dish);
        await LoadDataAsync();
        
        if (SelectedDish == null)
            ClearForm();
    }

    private async Task DeleteDishAsync()
    {
        if (SelectedDish != null)
        {
            await _blo.DeleteDishAsync(SelectedDish.Id);
            await LoadDataAsync();
            ClearForm();
        }
    }

    private async Task AddIngredientAsync()
    {
        if (SelectedDish != null && SelectedIngredientToAdd != null && IngredientQuantity > 0)
        {
            await _blo.AddDishIngredientAsync(SelectedDish.Id, SelectedIngredientToAdd.Id, IngredientQuantity);
            await LoadDishIngredientsAsync();
            await RecalculateAsync();
            IngredientQuantity = 0;
            SelectedIngredientToAdd = null;
        }
    }

    private async Task RemoveIngredientAsync()
    {
        if (SelectedDish != null && SelectedDishIngredient != null)
        {
            await _blo.RemoveDishIngredientAsync(SelectedDish.Id, SelectedDishIngredient.IngredientId);
            await LoadDishIngredientsAsync();
            await RecalculateAsync();
        }
    }

    private async Task RecalculateAsync()
    {
        if (SelectedDish != null)
        {
            var updatedDish = await _blo.RecalculateDishCostAndCaloriesAsync(SelectedDish.Id);
            TotalCalories = updatedDish.TotalCalories;
            TotalCost = updatedDish.TotalCost;
            
            // Update in the list
            var dish = Dishes.FirstOrDefault(d => d.Id == updatedDish.Id);
            if (dish != null)
            {
                dish.TotalCalories = updatedDish.TotalCalories;
                dish.TotalCost = updatedDish.TotalCost;
            }
        }
    }

    private void ClearForm()
    {
        SelectedDish = null;
        Name = string.Empty;
        Category = null;
        Description = null;
        Recipe = null;
        TotalCalories = 0;
        TotalCost = 0;
        SellingPrice = 0;
        IsActive = true;
        DishIngredients.Clear();
        IngredientQuantity = 0;
        SelectedIngredientToAdd = null;
    }
}
