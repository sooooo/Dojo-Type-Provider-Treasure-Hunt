﻿#r "System.Xml.Linq.dll"
#r "packages/FSharp.Data.2.0.0-beta2/lib/net40/FSharp.Data.dll"
open FSharp.Data

// ------------------------------------------------------------------
// WORD #1
//
// Use the WorldBank type provider to get all countries in the 
// "North America" region, then find country "c" with the smallest
// "Life expectancy at birth, total (years)" value in the year 2000
// and return the first two letters of the "c.Code" property
// ------------------------------------------------------------------

// Create connection to the WorldBank service
let wb = WorldBankData.GetDataContext()
// Get specific indicator for a specific country at a given year
wb.Countries.``Czech Republic``.Indicators.``Population (Total)``.[2000]
// Get a list of countries in a specified region
// wb.Regions.``Euro area``.Countries
let countries = wb.Regions.``North America``.Countries
let smallest = 
    countries 
    |> Seq.map (fun c -> 
        c.Name, 
        [ for y in 2000 .. 2005 -> c.Indicators.``Life expectancy at birth, total (years)``.[y] ])
    |> Seq.toArray

let byYear = 
    [ 2000 .. 2005 ]
    |> List.map (fun y -> 
        printfn "In %i " y
        countries |> Seq.iter (fun c -> printfn "%s,%f" c.Name c.Indicators.``Life expectancy at birth, total (years)``.[y])
        countries 
        |> Seq.minBy (fun c -> c.Indicators.``Life expectancy at birth, total (years)``.[y]))
//        |> fun c -> sprintf "%s" c.Name)

let smallest2 = 
    countries 
    |> Seq.minBy (fun c -> c.Indicators.``Life expectancy at birth, total (years)``.[2000])
    |> fun c -> c.Code.Substring(0,2)

//    |> Seq.map (fun c -> (c.Code, c.Indicators.``Life expectancy at birth, total (years)``.[2000]))
//    |> Seq.sortBy (fun (code,exp) -> exp)
//    |> Seq.nth 0
//    |> fun (code,exp) -> code.Substring(0,2)


let f x = x + 5
let data = [ 1 .. 10 ]
data |> List.map f
data |> List.map (fun x -> sprintf "Number %i" x)

let g x y = printfn "%s,%i" x y
42 |> g "Hello"

// ------------------------------------------------------------------
// WORD #2
//
// Read the RSS feed in "data/bbc.xml" using XmlProvider and return
// the last word of the title of an article that was published at
// 9:05am (the date does not matter, just hours & minutes)
// ------------------------------------------------------------------

// Create a type for working with XML documents based on a sample file
//type Sample = XmlProvider<"data/Writers.xml">
type Sample = XmlProvider<"data/bbc.xml">
let doc = Sample.GetSample()
let x = doc.Channel.Items
x |> Seq.filter (fun item -> (item.PubDate.Hour = 9) && (item.PubDate.Minute = 5))
  |> Seq.map (fun item -> item.Title.Split(' '))
  |> Seq.map (fun words -> 
      let last = words.Length
      words.[last-1])
  |> Seq.toArray


// ------------------------------------------------------------------
// WORD #3
//
// Get the ID of a director whose name contains "Haugerud" and then
// search for all movie credits where he appears. Then return the 
// second (last) word from the movie he directed (the resulting type
// has properties "Credits" and "Crew" - you need movie from the 
// Crew list (there is only one).
// ------------------------------------------------------------------

// Using The MovieDB REST API
// Make HTTP request to /3/search/person
let key = "6ce0ef5b176501f8c07c634dfa933cff"
//let name = "craig"
let name = "Haugerud"

//let data = 
//  Http.RequestString
//    ( "http://api.themoviedb.org/3/search/person",
//      query = [ ("query", "craig"); ("api_key", key) ],
//      headers = ["accept", "application/json"] )
let data = 
  Http.RequestString
    ( "http://api.themoviedb.org/3/search/person",
      query = [ ("query", name); ("api_key", key) ],
      headers = ["accept", "application/json"] )


// Parse result using JSON provider
// (using sample result to generate types)
type PersonSearch = JsonProvider<"data/personsearch.json">
let sample = PersonSearch.Parse(data)

let first = sample.Results |> Seq.head
first.Id

// Request URL: "http://api.themoviedb.org/3/person/<id>/movie_credits
// (You can remove the 'query' parameter because it is not needed here;
// you need to put the director's ID in place of the <id> part of the URL)

// Use JsonProvider with sample file "data/moviecredits.json" to parse


// ------------------------------------------------------------------
// WORD #4
//
// Use CsvProvider to parse the file "data/librarycalls.csv" which
// contains information about some PHP library calls (got it from the
// internet :-)). Note that the file uses ; as the separator.
//
// Then find row where 'params' is equal to 2 and 'count' is equal to 1
// and the 'name' column is longer than 6 characters. Return first such
// row and get the last word of the function name (which is separated
// by underscores). Make the word plural by adding 's' to the end.
// ------------------------------------------------------------------

// Demo - using CSV provider to read CSV file with custom separator
type Lib = CsvProvider<"data/mortalityny.tsv", Separators="\t">
// Read the sample file - explore the collection of rows in "lib.Data"
let lib = new Lib()


// ------------------------------------------------------------------
// WORD #5
//
// Use Freebase type provider to find chemical element with 
// "Atomic number" equal to 36 and then return the 3rd and 2nd 
// letter from the *end* of its name (that is 5th and 6th letter
// from the start).
// ------------------------------------------------------------------

// Create connection to the Freebase service
let fb = FreebaseData.GetDataContext()

// Query stars in the science & technology category
// (You'll need "Science and Technology" and then "Chemistry")
query { for e in fb.``Science and Technology``.Astronomy.Stars do 
        where e.Distance.HasValue
        select (e.Name, e.Distance) } 
|> Seq.toList

// ------------------------------------------------------------------
// WORD #6
//
// Use CsvProvider to load the Titanic data set from "data/Titanic.csv"
// (using the default column separator) and find the name of a female
// passenger who was 19 years old and embarked in the prot marked "Q"
// Then return Substring(19, 3) from her name.
// ------------------------------------------------------------------

// Use CsvProvider with the "data/titanic.csv" file as a sample


// ------------------------------------------------------------------
// WORD #7
//
// Using the same RSS feed as in Word #2 ("data/bbc.xml"), find
// item that contains "Duran Duran" in the title and return the
// 14th word from its description (split the description using ' '
// as the separator and get item at index 13).
// ------------------------------------------------------------------

// (...)