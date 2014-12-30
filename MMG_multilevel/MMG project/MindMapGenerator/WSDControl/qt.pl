use WordNet::QueryData;

my $wn = WordNet::QueryData->new;

print $wn->frequency('dog#n#1');
print "\n";
print $wn->frequency('dog#n#2');

