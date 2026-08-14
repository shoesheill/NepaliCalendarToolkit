SEEDER_PROJECT  := tools/NepaliCalendarDataSeeder/NepaliCalendarDataSeeder.csproj

.PHONY: seed build clean

seed:
	dotnet run -c Release --project "$(SEEDER_PROJECT)"

build:
	dotnet build -c Release --project "$(SEEDER_PROJECT)"

clean:
	rm -rf tools/NepaliCalendarDataSeeder/bin tools/NepaliCalendarDataSeeder/obj
