
ABOUT
=====

This tool compies metadata from an unused iTunes library to a used library.
I made it becouse all my song-ratings and number-of-plays where gone when I reinstalled my computer. So I used the old iTunes-metadata-xml and used the API route the metadata to the new (living) iTunes.

MORE INFO
=========

Itunes has a binary library to save all the song names, ratings, etc. You can interact with this library with a c# interface, therfore you need a running iTunes.
Now, iTunes provides also an xml document which is eazy readable.
My program streams the info from whatever iTunes-XML-file to the local real Library.


<pre>
  ______          ______
 |      \        /      \
 |  Old  |      / current\
 |  .xml | ==> |  itunes  |
 |_______|      \________/
 
</pre>

* Make the tool find the metadata xml (paste the path to it on the first line)
* select the song in itunes that can be affected
* Click th big sync button
* to see the progres, keep ckilkking on the progressbar.


```csharp

// Here is the essence how two songs are comared: (c#)

// ' livingTrack (from current iTunes) '  ==  ' oldTrack (from other iTunes XML) '
livingTrack.PlayedCount = Math.Max(livingTrack.PlayedCount, oldTrack.PlayCount);
if (!oldTrack.RatingComputed && oldTrack.Rating != 0)
{
    livingTrack.Rating = Math.Max(oldTrack.Rating, livingTrack.Rating);
}

```

_Made By Emile Sonneveld_