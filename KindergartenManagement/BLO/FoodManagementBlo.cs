using KindergartenManagement.DAO;
using KindergartenManagement.DTO;

namespace KindergartenManagement.BLO;

public interface IFoodManagementBlo
{
    // Supplier
    Task<List<Supplier>> GetAllSuppliersAsync();
    Task<Supplier?> GetSupplierByIdAsync(Guid id);
    Task<Supplier> SaveSupplierAsync(Supplier supplier);
    Task<bool> DeleteSupplierAsync(Guid id);

    // Ingredient
    Task<List<Ingredient>> GetAllIngredientsAsync();
    Task<List<Ingredient>> GetIngredientsBySupplierAsync(Guid supplierId);
    Task<Ingredient?> GetIngredientByIdAsync(Guid id);
    Task<Ingredient> SaveIngredientAsync(Ingredient ingredient);
    Task<bool> DeleteIngredientAsync(Guid id);

    // Dish
    Task<List<Dish>> GetAllDishesAsync();
    Task<Dish?> GetDishByIdAsync(Guid id);
    Task<Dish?> GetDishWithIngredientsAsync(Guid id);
    Task<Dish> SaveDishAsync(Dish dish);
    Task<bool> DeleteDishAsync(Guid id);
    Task<Dish> RecalculateDishCostAndCaloriesAsync(Guid dishId);

    // DishIngredient
    Task<List<DishIngredient>> GetDishIngredientsAsync(Guid dishId);
    Task<DishIngredient> AddDishIngredientAsync(Guid dishId, Guid ingredientId, decimal quantity);
    Task<bool> RemoveDishIngredientAsync(Guid dishId, Guid ingredientId);
    Task<bool> UpdateDishIngredientQuantityAsync(Guid dishId, Guid ingredientId, decimal quantity);

    // Menu
    Task<List<Menu>> GetAllMenusAsync();
    Task<List<Menu>> GetMenusByDayOfWeekAsync(int dayOfWeek);
    Task<Menu?> GetMenuByIdAsync(Guid id);
    Task<Menu?> GetMenuWithDishesAsync(Guid id);
    Task<Menu> SaveMenuAsync(Menu menu);
    Task<bool> DeleteMenuAsync(Guid id);

    // MenuDish
    Task<List<MenuDish>> GetMenuDishesAsync(Guid menuId);
    Task<MenuDish> AddMenuDishAsync(Guid menuId, Guid dishId);
    Task<bool> RemoveMenuDishAsync(Guid menuId, Guid dishId);

    // MealTicket
    Task<List<MealTicket>> GetMealTicketsByDateAsync(DateTime date);
    Task<List<MealTicket>> GetMealTicketsByStudentAsync(Guid studentId);
    Task<List<MealTicket>> GetMealTicketsByMenuAsync(Guid menuId);
    Task<MealTicket> SaveMealTicketAsync(MealTicket mealTicket);
    Task<bool> DeleteMealTicketAsync(Guid id);
    Task<int> GenerateMealTicketsForMenuAsync(Guid menuId, DateTime date);
}

public class FoodManagementBlo : IFoodManagementBlo
{
    private readonly IFoodManagementDao _dao;

    public FoodManagementBlo(IFoodManagementDao dao)
    {
        _dao = dao;
    }

    // Supplier
    public async Task<List<Supplier>> GetAllSuppliersAsync()
    {
        return await _dao.GetAllSuppliersAsync();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(Guid id)
    {
        return await _dao.GetSupplierByIdAsync(id);
    }

    public async Task<Supplier> SaveSupplierAsync(Supplier supplier)
    {
        if (supplier.Id == Guid.Empty)
        {
            supplier.Id = Guid.NewGuid();
            return await _dao.AddSupplierAsync(supplier);
        }
        return await _dao.UpdateSupplierAsync(supplier);
    }

    public async Task<bool> DeleteSupplierAsync(Guid id)
    {
        return await _dao.DeleteSupplierAsync(id);
    }

    // Ingredient
    public async Task<List<Ingredient>> GetAllIngredientsAsync()
    {
        return await _dao.GetAllIngredientsAsync();
    }

    public async Task<List<Ingredient>> GetIngredientsBySupplierAsync(Guid supplierId)
    {
        return await _dao.GetIngredientsBySupplerAsync(supplierId);
    }

    public async Task<Ingredient?> GetIngredientByIdAsync(Guid id)
    {
        return await _dao.GetIngredientByIdAsync(id);
    }

    public async Task<Ingredient> SaveIngredientAsync(Ingredient ingredient)
    {
        if (ingredient.Id == Guid.Empty)
        {
            ingredient.Id = Guid.NewGuid();
            return await _dao.AddIngredientAsync(ingredient);
        }
        return await _dao.UpdateIngredientAsync(ingredient);
    }

    public async Task<bool> DeleteIngredientAsync(Guid id)
    {
        return await _dao.DeleteIngredientAsync(id);
    }

    // Dish
    public async Task<List<Dish>> GetAllDishesAsync()
    {
        return await _dao.GetAllDishesAsync();
    }

    public async Task<Dish?> GetDishByIdAsync(Guid id)
    {
        return await _dao.GetDishByIdAsync(id);
    }

    public async Task<Dish?> GetDishWithIngredientsAsync(Guid id)
    {
        return await _dao.GetDishWithIngredientsAsync(id);
    }

    public async Task<Dish> SaveDishAsync(Dish dish)
    {
        if (dish.Id == Guid.Empty)
        {
            dish.Id = Guid.NewGuid();
            return await _dao.AddDishAsync(dish);
        }
        return await _dao.UpdateDishAsync(dish);
    }

    public async Task<bool> DeleteDishAsync(Guid id)
    {
        return await _dao.DeleteDishAsync(id);
    }

    public async Task<Dish> RecalculateDishCostAndCaloriesAsync(Guid dishId)
    {
        var dish = await _dao.GetDishWithIngredientsAsync(dishId);
        if (dish == null)
            throw new InvalidOperationException("Dish not found");

        decimal totalCalories = 0;
        decimal totalCost = 0;

        foreach (var dishIngredient in dish.DishIngredients)
        {
            if (dishIngredient.Ingredient != null)
            {
                // Calculate calories: (CaloriesPer100g * Quantity) / 100
                totalCalories += (dishIngredient.Ingredient.CaloriesPer100g * dishIngredient.Quantity) / 100;

                // Calculate cost: (UnitPrice * Quantity) / unit conversion
                // Assuming quantity is in grams and unit price is per unit (e.g., per kg = 1000g)
                totalCost += (dishIngredient.Ingredient.UnitPrice * dishIngredient.Quantity) / 1000;
            }
        }

        dish.TotalCalories = Math.Round(totalCalories, 2);
        dish.TotalCost = Math.Round(totalCost, 2);

        return await _dao.UpdateDishAsync(dish);
    }

    // DishIngredient
    public async Task<List<DishIngredient>> GetDishIngredientsAsync(Guid dishId)
    {
        return await _dao.GetDishIngredientsAsync(dishId);
    }

    public async Task<DishIngredient> AddDishIngredientAsync(Guid dishId, Guid ingredientId, decimal quantity)
    {
        var dishIngredient = new DishIngredient
        {
            DishId = dishId,
            IngredientId = ingredientId,
            Quantity = quantity
        };

        var result = await _dao.AddDishIngredientAsync(dishIngredient);
        
        // Recalculate dish totals
        await RecalculateDishCostAndCaloriesAsync(dishId);
        
        return result;
    }

    public async Task<bool> RemoveDishIngredientAsync(Guid dishId, Guid ingredientId)
    {
        var result = await _dao.DeleteDishIngredientAsync(dishId, ingredientId);
        
        if (result)
        {
            // Recalculate dish totals
            await RecalculateDishCostAndCaloriesAsync(dishId);
        }
        
        return result;
    }

    public async Task<bool> UpdateDishIngredientQuantityAsync(Guid dishId, Guid ingredientId, decimal quantity)
    {
        var result = await _dao.UpdateDishIngredientQuantityAsync(dishId, ingredientId, quantity);
        
        if (result)
        {
            // Recalculate dish totals
            await RecalculateDishCostAndCaloriesAsync(dishId);
        }
        
        return result;
    }

    // Menu
    public async Task<List<Menu>> GetAllMenusAsync()
    {
        return await _dao.GetAllMenusAsync();
    }

    public async Task<List<Menu>> GetMenusByDayOfWeekAsync(int dayOfWeek)
    {
        return await _dao.GetMenusByDayOfWeekAsync(dayOfWeek);
    }

    public async Task<Menu?> GetMenuByIdAsync(Guid id)
    {
        return await _dao.GetMenuByIdAsync(id);
    }

    public async Task<Menu?> GetMenuWithDishesAsync(Guid id)
    {
        return await _dao.GetMenuWithDishesAsync(id);
    }

    public async Task<Menu> SaveMenuAsync(Menu menu)
    {
        if (menu.Id == Guid.Empty)
        {
            menu.Id = Guid.NewGuid();
            return await _dao.AddMenuAsync(menu);
        }
        return await _dao.UpdateMenuAsync(menu);
    }

    public async Task<bool> DeleteMenuAsync(Guid id)
    {
        return await _dao.DeleteMenuAsync(id);
    }

    // MenuDish
    public async Task<List<MenuDish>> GetMenuDishesAsync(Guid menuId)
    {
        return await _dao.GetMenuDishesAsync(menuId);
    }

    public async Task<MenuDish> AddMenuDishAsync(Guid menuId, Guid dishId)
    {
        var menuDish = new MenuDish
        {
            MenuId = menuId,
            DishId = dishId
        };

        return await _dao.AddMenuDishAsync(menuDish);
    }

    public async Task<bool> RemoveMenuDishAsync(Guid menuId, Guid dishId)
    {
        return await _dao.DeleteMenuDishAsync(menuId, dishId);
    }

    // MealTicket
    public async Task<List<MealTicket>> GetMealTicketsByDateAsync(DateTime date)
    {
        return await _dao.GetMealTicketsByDateAsync(date);
    }

    public async Task<List<MealTicket>> GetMealTicketsByStudentAsync(Guid studentId)
    {
        return await _dao.GetMealTicketsByStudentAsync(studentId);
    }

    public async Task<List<MealTicket>> GetMealTicketsByMenuAsync(Guid menuId)
    {
        return await _dao.GetMealTicketsByMenuAsync(menuId);
    }

    public async Task<MealTicket> SaveMealTicketAsync(MealTicket mealTicket)
    {
        if (mealTicket.Id == Guid.Empty)
        {
            mealTicket.Id = Guid.NewGuid();
            return await _dao.AddMealTicketAsync(mealTicket);
        }
        return await _dao.UpdateMealTicketAsync(mealTicket);
    }

    public async Task<bool> DeleteMealTicketAsync(Guid id)
    {
        return await _dao.DeleteMealTicketAsync(id);
    }

    public async Task<int> GenerateMealTicketsForMenuAsync(Guid menuId, DateTime date)
    {
        // Business logic: Only generate meal tickets for students who are present
        return await _dao.GenerateMealTicketsForMenuAsync(menuId, date);
    }
}
