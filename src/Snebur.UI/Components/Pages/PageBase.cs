using System.Reflection;
using Snebur.Core.Exceptions;
using Snebur.UI.Core;

namespace Snebur.UI.Components.Pages;

public abstract class PageBase : ComponentBase
{
    [Parameter]
    public string? Culture { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var viewModel = TryGetViewModel();
        if (viewModel is not null)
        {
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        EnsureDependencies();
    }

    private ViewModelBase? TryGetViewModel()
    {
        var flags = ReflectionUtils.AllInstanceBindingFlags;
        var type = this.GetType();

        return type.GetProperties(flags)
            .FirstOrDefault(p => p.PropertyType.IsSubclassOf(typeof(ViewModelBase)))
            ?.GetValue(this) as ViewModelBase
            ?? type.GetFields(flags)
                .FirstOrDefault(p => p.FieldType.IsSubclassOf(typeof(ViewModelBase)))
                ?.GetValue(this) as ViewModelBase;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e == null)
            return;
         
        if (OperatingSystem.IsBrowser())
        {
            // In Blazor WebAssembly, calling StateHasChanged is usually unnecessary
            return;
        }
        InvokeAsync(StateHasChanged);
    }

    private void EnsureDependencies()
    {
        var flags = ReflectionUtils.AllInstanceBindingFlags;
        var injectProperties = GetType()
            .GetProperties(flags)
            .Where(x => x.GetCustomAttribute<InjectAttribute>() != null);

        foreach (var property in injectProperties)
        {
            if (property.IsNullable())
            {
                continue;
            }

            var propertyValue = property.GetValue(this);
            if (propertyValue is null)
            {
                throw new InvalidOperationException(
                    $"The required service '{property.PropertyType.FullName}' was not injected into the property '{property.Name}' on component '{GetType().FullName}'. " +
                    "Make sure it is registered in the DI container and marked with the [Inject] attribute.");
            }
        }
    }
}
