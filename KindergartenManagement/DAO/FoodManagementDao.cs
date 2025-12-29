using Microsoft.EntityFrameworkCore;
using KindergartenManagement.DTO;

namespace KindergartenManagement.DAO;

public interface IFoodManagementDao
{
    // Supplier methods
    Task<List<Supplier>> GetAllSuppliersAsync();
    Task<Supplier?> GetSupplierByIdAsync(Guid id);
    Task<Supplier> AddSupplierAsync(Supplier supplier);
    Task<Supplier> UpdateSupplierAsync(Supplier supplier);
    Task<bool> DeleteSupplierAsync(Guid id);

    // Ingredient methods
    Task<List<Ingredient>> GetAllIngredientsAsync();
    Task<List<Ingredient>> GetIngredientsBySupplerAsync(Guid supplierId);
    Task<Ingredient?> GetIngredientByIdAsync(Guid id);
    Task<Ingredient> AddIngredientAsync(Ingredient ingredient);
    Task<Ingredient> UpdateIngredientAsync(Ingredient ingredient);
    Task<bool> DeleteIngredientAsync(Guid id);

    // Dish methods
    Task<List<Dish>> GetAllDishesAsync();
    Task<Dish?> GetDishByIdAsync(Guid id);
    Task<Dish?> GetDishWithIngredientsAsync(Guid id);
    Task<Dish> AddDishAsync(Dish dish);
    Task<Dish> UpdateDishAsync(Dish dish);
    Task<bool> DeleteDishAsync(Guid id);

    // DishIngredient methods
    Task<List<DishIngredient>> GetDishIngredientsAsync(Guid dishId);
    Task<DishIngredient> AddDishIngredientAsync(DishIngredient dishIngredient);
    Task<bool> DeleteDishIngredientAsync(Guid dishId, Guid ingredientId);
    Task<bool> UpdateDishIngredientQuantityAsync(Guid dishId, Guid ingredientId, decimal quantity);

    // Menu methods
    Task<List<Menu>> GetAllMenusAsync();
    Task<List<Menu>> GetMenusByDayOfWeekAsync(int dayOfWeek);
    Task<Menu?> GetMenuByIdAsync(Guid id);
    Task<Menu?> GetMenuWithDishesAsync(Guid id);
    Task<Menu> AddMenuAsync(Menu menu);
    Task<Menu> UpdateMenuAsync(Menu menu);
    Task<bool> DeleteMenuAsync(Guid id);

    // MenuDish methods
    Task<List<MenuDish>> GetMenuDishesAsync(Guid menuId);
    Task<MenuDish> AddMenuDishAsync(MenuDish menuDish);
    Task<bool> DeleteMenuDishAsync(Guid menuId, Guid dishId);

    // MealTicket methods
    Task<List<MealTicket>> GetMealTicketsByDateAsync(DateTime date);
    Task<List<MealTicket>> GetMealTicketsByStudentAsync(Guid studentId);
    Task<List<MealTicket>> GetMealTicketsByMenuAsync(Guid menuId);
    Task<MealTicket> AddMealTicketAsync(MealTicket mealTicket);
    Task<MealTicket> UpdateMealTicketAsync(MealTicket mealTicket);
    Task<bool> DeleteMealTicketAsync(Guid id);
    Task<int> GenerateMealTicketsForMenuAsync(Guid menuId, DateTime date);
}

public class FoodManagementDao : IFoodManagementDao
{
    private readonly KindergartenDbContext _context;

    public FoodManagementDao(KindergartenDbContext context)
    {
        _context = context;
    }

    // Supplier methods
    public async Task<List<Supplier>> GetAllSuppliersAsync()
    {
        return await _context.Suppliers.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<Supplier?> GetSupplierByIdAsync(Guid id)
    {
        return await _context.Suppliers.FindAsync(id);
    }

    public async Task<Supplier> AddSupplierAsync(Supplier supplier)
    {
        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        return supplier;
    }

    public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
    {
        supplier.UpdatedAt = DateTime.Now;
        _context.Suppliers.Update(supplier);
        await _context.SaveChangesAsync();
        return supplier;
    }

    public async Task<bool> DeleteSupplierAsync(Guid id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null) return false;

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();
        return true;
    }

    // Ingredient methods
    public async Task<List<Ingredient>> GetAllIngredientsAsync()
    {
        return await _context.Ingredients
            .Include(i => i.Supplier)
            .OrderBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<List<Ingredient>> GetIngredientsBySupplerAsync(Guid supplierId)
    {
        return await _context.Ingredients
            .Where(i => i.SupplierId == supplierId)
            .OrderBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<Ingredient?> GetIngredientByIdAsync(Guid id)
    {
        return await _context.Ingredients
            .Include(i => i.Supplier)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Ingredient> AddIngredientAsync(Ingredient ingredient)
    {
        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();
        return ingredient;
    }

    public async Task<Ingredient> UpdateIngredientAsync(Ingredient ingredient)
    {
        ingredient.UpdatedAt = DateTime.Now;
        _context.Ingredients.Update(ingredient);
        await _context.SaveChangesAsync();
        return ingredient;
    }

    public async Task<bool> DeleteIngredientAsync(Guid id)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (ingredient == null) return false;

        _context.Ingredients.Remove(ingredient);
        await _context.SaveChangesAsync();
        return true;
    }

    // Dish methods
    public async Task<List<Dish>> GetAllDishesAsync()
    {
        return await _context.Dishes
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<Dish?> GetDishByIdAsync(Guid id)
    {
        return await _context.Dishes.FindAsync(id);
    }

    public async Task<Dish?> GetDishWithIngredientsAsync(Guid id)
    {
        return await _context.Dishes
            .Include(d => d.DishIngredients)
            .ThenInclude(di => di.Ingredient)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Dish> AddDishAsync(Dish dish)
    {
        _context.Dishes.Add(dish);
        await _context.SaveChangesAsync();
        return dish;
    }

    public async Task<Dish> UpdateDishAsync(Dish dish)
    {
        dish.UpdatedAt = DateTime.Now;
        _context.Dishes.Update(dish);
        await _context.SaveChangesAsync();
        return dish;
    }

    public async Task<bool> DeleteDishAsync(Guid id)
    {
        var dish = await _context.Dishes.FindAsync(id);
        if (dish == null) return false;

        _context.Dishes.Remove(dish);
        await _context.SaveChangesAsync();
        return true;
    }

    // DishIngredient methods
    public async Task<List<DishIngredient>> GetDishIngredientsAsync(Guid dishId)
    {
        return await _context.DishIngredients
            .Where(di => di.DishId == dishId)
            .Include(di => di.Ingredient)
            .ToListAsync();
    }

    public async Task<DishIngredient> AddDishIngredientAsync(DishIngredient dishIngredient)
    {
        _context.DishIngredients.Add(dishIngredient);
        await _context.SaveChangesAsync();
        return dishIngredient;
    }

    public async Task<bool> DeleteDishIngredientAsync(Guid dishId, Guid ingredientId)
    {
        var dishIngredient = await _context.DishIngredients
            .FirstOrDefaultAsync(di => di.DishId == dishId && di.IngredientId == ingredientId);
        
        if (dishIngredient == null) return false;

        _context.DishIngredients.Remove(dishIngredient);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateDishIngredientQuantityAsync(Guid dishId, Guid ingredientId, decimal quantity)
    {
        var dishIngredient = await _context.DishIngredients
            .FirstOrDefaultAsync(di => di.DishId == dishId && di.IngredientId == ingredientId);
        
        if (dishIngredient == null) return false;

        dishIngredient.Quantity = quantity;
        await _context.SaveChangesAsync();
        return true;
    }

    // Menu methods
    public async Task<List<Menu>> GetAllMenusAsync()
    {
        return await _context.Menus
            .OrderBy(m => m.DayOfWeek)
            .ThenBy(m => m.MealType)
            .ToListAsync();
    }

    public async Task<List<Menu>> GetMenusByDayOfWeekAsync(int dayOfWeek)
    {
        return await _context.Menus
            .Where(m => m.DayOfWeek == dayOfWeek)
            .OrderBy(m => m.MealType)
            .ToListAsync();
    }

    public async Task<Menu?> GetMenuByIdAsync(Guid id)
    {
        return await _context.Menus.FindAsync(id);
    }

    public async Task<Menu?> GetMenuWithDishesAsync(Guid id)
    {
        return await _context.Menus
            .Include(m => m.MenuDishes)
            .ThenInclude(md => md.Dish)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Menu> AddMenuAsync(Menu menu)
    {
        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();
        return menu;
    }

    public async Task<Menu> UpdateMenuAsync(Menu menu)
    {
        menu.UpdatedAt = DateTime.Now;
        _context.Menus.Update(menu);
        await _context.SaveChangesAsync();
        return menu;
    }

    public async Task<bool> DeleteMenuAsync(Guid id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu == null) return false;

        _context.Menus.Remove(menu);
        await _context.SaveChangesAsync();
        return true;
    }

    // MenuDish methods
    public async Task<List<MenuDish>> GetMenuDishesAsync(Guid menuId)
    {
        return await _context.MenuDishes
            .Where(md => md.MenuId == menuId)
            .Include(md => md.Dish)
            .ToListAsync();
    }

    public async Task<MenuDish> AddMenuDishAsync(MenuDish menuDish)
    {
        _context.MenuDishes.Add(menuDish);
        await _context.SaveChangesAsync();
        return menuDish;
    }

    public async Task<bool> DeleteMenuDishAsync(Guid menuId, Guid dishId)
    {
        var menuDish = await _context.MenuDishes
            .FirstOrDefaultAsync(md => md.MenuId == menuId && md.DishId == dishId);
        
        if (menuDish == null) return false;

        _context.MenuDishes.Remove(menuDish);
        await _context.SaveChangesAsync();
        return true;
    }

    // MealTicket methods
    public async Task<List<MealTicket>> GetMealTicketsByDateAsync(DateTime date)
    {
        return await _context.MealTickets
            .Where(mt => mt.Date.Date == date.Date)
            .Include(mt => mt.Student)
            .Include(mt => mt.Menu)
            .ToListAsync();
    }

    public async Task<List<MealTicket>> GetMealTicketsByStudentAsync(Guid studentId)
    {
        return await _context.MealTickets
            .Where(mt => mt.StudentId == studentId)
            .Include(mt => mt.Menu)
            .OrderByDescending(mt => mt.Date)
            .ToListAsync();
    }

    public async Task<List<MealTicket>> GetMealTicketsByMenuAsync(Guid menuId)
    {
        return await _context.MealTickets
            .Where(mt => mt.MenuId == menuId)
            .Include(mt => mt.Student)
            .ToListAsync();
    }

    public async Task<MealTicket> AddMealTicketAsync(MealTicket mealTicket)
    {
        _context.MealTickets.Add(mealTicket);
        await _context.SaveChangesAsync();
        return mealTicket;
    }

    public async Task<MealTicket> UpdateMealTicketAsync(MealTicket mealTicket)
    {
        mealTicket.UpdatedAt = DateTime.Now;
        _context.MealTickets.Update(mealTicket);
        await _context.SaveChangesAsync();
        return mealTicket;
    }

    public async Task<bool> DeleteMealTicketAsync(Guid id)
    {
        var mealTicket = await _context.MealTickets.FindAsync(id);
        if (mealTicket == null) return false;

        _context.MealTickets.Remove(mealTicket);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GenerateMealTicketsForMenuAsync(Guid menuId, DateTime date)
    {
        // Get menu
        var menu = await _context.Menus.FindAsync(menuId);
        if (menu == null) return 0;

        // Get all present students for this date
        var presentStudents = await _context.Attendances
            .Where(a => a.Date.Date == date.Date && a.Status == "Present")
            .Select(a => a.StudentId)
            .ToListAsync();

        int count = 0;
        foreach (var studentId in presentStudents)
        {
            // Check if meal ticket already exists
            var exists = await _context.MealTickets
                .AnyAsync(mt => mt.StudentId == studentId && mt.MenuId == menuId && mt.Date.Date == date.Date);

            if (!exists)
            {
                var mealTicket = new MealTicket
                {
                    StudentId = studentId,
                    MenuId = menuId,
                    Date = date,
                    IsConsumed = false
                };
                _context.MealTickets.Add(mealTicket);
                count++;
            }
        }

        await _context.SaveChangesAsync();
        return count;
    }
}
