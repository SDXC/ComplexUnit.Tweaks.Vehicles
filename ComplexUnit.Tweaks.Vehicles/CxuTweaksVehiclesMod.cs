using Mafi;
using Mafi.Collections;
using Mafi.Core.Buildings.OreSorting;
using Mafi.Core.Game;
using Mafi.Core.Mods;
using Mafi.Core.Prototypes;
using Mafi.Core.Vehicles.Excavators;
using Mafi.Core.Vehicles.Trucks;
using Mafi.Serialization;


namespace ComplexUnit.Tweaks.Vehicles
{
    public sealed class CxuTweaksVehiclesMod : IMod
    {
        private string Name => nameof(CxuTweaksVehiclesMod);
        public int Version => 1;

        public ModManifest Manifest { get; }
        public bool IsUiOnly => false;
        public Option<IConfig> ModConfig => Option<IConfig>.None;
        public ModJsonConfig JsonConfig { get; }



        public bool EnableCustomTruckCapacity => this.JsonConfig.GetBool("enable_custom_truck_capacity");
        public int TruckCapacityMultiplier => this.JsonConfig.GetInt("truck_capacity_multiplier");
        public bool EnableCustomExcavatorHaulCapacity => this.JsonConfig.GetBool("enable_custom_excavator_haul_capacity");
        public int ExcavatorCapacityMultiplier => this.JsonConfig.GetInt("excavator_capacity_multiplier");



        public CxuTweaksVehiclesMod(ModManifest manifest)
        {
            this.Manifest = manifest;
            this.JsonConfig = new ModJsonConfig(this);

            Log.Info($"{this.Name}: Mod loaded!");
        }



        public void EarlyInit(DependencyResolver resolver)
        {
            return;
        }

        public void Initialize(DependencyResolver resolver, bool gameWasLoaded)
        {
            return;
        }

        public void MigrateJsonConfig(VersionSlim savedVersion, Dict<string, object> savedValues)
        {
            return;
        }

        public void RegisterDependencies(DependencyResolverBuilder depBuilder, ProtosDb protosDb, bool gameWasLoaded)
        {
            return;
        }

        public void RegisterPrototypes(ProtoRegistrator registrator)
        {
            if (this.EnableCustomTruckCapacity)
            {
                Log.Info($"{this.Name}: Registering new truck capacities...");

                foreach (TruckProto truck in registrator.PrototypesDb.All<TruckProto>())
                {
                    TruckProto proto = registrator.PrototypesDb.Get<TruckProto>(truck.Id).Value;

                    ReflectionUtils.SetField<TruckProto>(truck, nameof(proto.CapacityBase), proto.CapacityBase * this.TruckCapacityMultiplier);
                }

                Log.Info($"{this.Name}: Finished registering new truck capacities.");
                Log.Info($"{this.Name}: Scale sorter properties...");

                foreach (OreSortingPlantProto sorter in registrator.PrototypesDb.All<OreSortingPlantProto>())
                {
                    OreSortingPlantProto proto = registrator.PrototypesDb.Get<OreSortingPlantProto>(sorter.Id).Value;

                    ReflectionUtils.SetField<OreSortingPlantProto>(sorter, nameof(proto.InputBufferCapacity), proto.InputBufferCapacity * this.TruckCapacityMultiplier);
                    ReflectionUtils.SetField<OreSortingPlantProto>(sorter, nameof(proto.OutputBuffersCapacity), proto.OutputBuffersCapacity * this.TruckCapacityMultiplier);
                    ReflectionUtils.SetField<OreSortingPlantProto>(sorter, "<" + nameof(proto.QuantityPerDuration) + ">k__BackingField", proto.QuantityPerDuration * this.TruckCapacityMultiplier);
                }

                Log.Info($"{this.Name}: Finished scaling sorter properties.");
            }

            if (this.EnableCustomExcavatorHaulCapacity)
            {
                Log.Info($"{this.Name}: Registering new excavator haul capacities...");

                foreach (ExcavatorProto excavator in registrator.PrototypesDb.All<ExcavatorProto>())
                {
                    ExcavatorProto proto = registrator.PrototypesDb.Get<ExcavatorProto>(excavator.Id).Value;

                    ReflectionUtils.SetField<ExcavatorProto>(proto, nameof(proto.Capacity), proto.Capacity * this.ExcavatorCapacityMultiplier);
                }

                Log.Info($"{this.Name}: Finished registering new excavator haul capacities.");
            }
        }

        public void Dispose()
        {
            return;
        }
    }
}
