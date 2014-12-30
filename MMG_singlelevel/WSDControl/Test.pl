$WNpath = "..\\dict";
$Algorithm = "WordNet::SenseRelate::Algorithm::Global";
$Config = ".\\config\\config-lesk.conf";
$Measure = "WordNet::Similarity::lesk";

Init();
#my $input="?-?-like#v#5-v|?-?-mary#n#1-n";
#print Disambigute($input);
print IsPerson("mary#n#1");

#--------------------------------------------------------------------------

use strict;
use WordNet::SenseRelate::Word;
use WordNet::SenseRelate::Tools;

# Get algorithm modules
use WordNet::SenseRelate::Algorithm::Local;
use WordNet::SenseRelate::Algorithm::Global;

# Get measure modules
use WordNet::Similarity::path;
use WordNet::Similarity::lch;
use WordNet::Similarity::wup;
use WordNet::Similarity::res;
use WordNet::Similarity::hso;
use WordNet::Similarity::jcn;
use WordNet::Similarity::lesk;
use WordNet::Similarity::lin;
use WordNet::Similarity::vector;
use WordNet::Similarity::vector_pairs;

# Public variales
our $WNpath = undef;
our $Algorithm = undef;
our $Measure = undef;
our $Config = undef;

# Private variales
my $wntools = undef;
my $module = undef;

#Methods

sub Init {
    
   # Load WordNet::SenseRelate::Tools:
   $wntools = WordNet::SenseRelate::Tools->new($WNpath);
   die "Unable to create WordNetTools object.\n" unless defined $wntools;
   
   # Set the measure
   my $config={};
   $config->{measure} = $Measure;
   $config->{measureconfig} = $Config;

   # Load Disambiguation Algorithm module
   $module = $Algorithm->new($wntools, 0, $config);
   die "Unable to create algorithm object.\n" unless defined $module;

}

sub UpdateMeasure {
    
   # Set the measure
   my $config={};
   $config->{measure} = $Measure;
   $config->{measureconfig} = $Config;

   # Load Disambiguation Algorithm module
   $module = $Algorithm->new($wntools, 0, $config);
   die "Unable to create algorithm object.\n" unless defined $module;

}

sub IsPerson
{
    my $word=shift;
    return 1 if $word eq "person#n#1";
    my $wn=$wntools->{wn};
    while()
    {
        my @res = $wn->querySense($word,"hypes");
        return 0 if scalar(@res) == 0;
        $word = $res[0];
        return 1 if $word eq "person#n#1";
    }
}

sub GetNounFromVerb
{
      my @words1 = shift;
      my $words=$words1[0];
      my $wn=shift;
      my @nouns;
      my $word;
      my $j=0;
      foreach my $k (0 .. $#{$words1[0]})
      {
        @nouns[$j]="";
        $word=$words->[$k];
   	my @deri=$wn->queryWord($word, "deri");
   	if(@deri > 0)
   	{
   	   $nouns[$j]= GetSenseFromDERI([@deri],$word);
   	}
   	else
   	{
   	   my @synset=$wn->querySense($word, "syns");
       loop:  foreach my $i (0 .. $#synset)
   	   {
   		if($synset[$i] ne $word)
   		{
   		    @deri=$wn->queryWord($synset[$i], "deri");
   		    if(@deri > 0)
   		    {
   		       $nouns[$j]= GetSenseFromDERI([@deri],$word);
   		       last loop;
   		    }
   		}
   	   }
   	}
   	if($nouns[$j] eq "")
   	{
   	   $nouns[$j]=$word;
   	}
   	$j=$j+1;
      }
      return @nouns;
}

sub GetSenseFromDERI
{
      my @deri1 = shift;
      my $deri=$deri1[0];
      my $origin = shift;
      my $score = 0;
      my $word = "";
      my $snum = 0;
 l1:  foreach my $i (0 .. $#{$deri1[0]})
      {
         my $token=$deri->[$i];
         my $wpart = substr($token,0,index($token,"#"));
         my $str3=substr($wpart,-4,4);
         my $str1=substr($wpart,-3,3);
         my $str2=substr($wpart,-2,2);
         my $currsnum = int substr($token,rindex($token,"#")+1);
         my $currpos=substr($token,index($token,"#")+1,1);
         next l1 if($currpos ne "n");
         if( ($str1 eq "ing") && ( $score<10 || ($score=10 && $currsnum<$snum) ) )
         {
            $score=10;
            $word=$token;
            $snum=$currsnum;
         }
         elsif( ($str1 eq "ion") && ( $score<8 || ($score=8 && $currsnum<$snum) ) )
         {
            $score=8;
            $word=$token;
            $snum=$currsnum;
         }
         elsif( ($str3 eq "ness") && ( $score<8 || ($score=8 && $currsnum<$snum) ) )
         {
            $score=8;
            $word=$token;
            $snum=$currsnum;
         }
         elsif( ($str3 eq "ment") && ( $score<8 || ($score=8 && $currsnum<$snum) ) )
         {
            $score=8;
            $word=$token;
            $snum=$currsnum;
         }
         elsif( ($str2 eq "th") && ( $score<8 || ($score=8 && $currsnum<$snum) ) )
         {
            $score=8;
            $word=$token;
            $snum=$currsnum;
         }
         elsif( ($str2 eq "er" || $str2 eq "or") && ( $score<6 || ($score=6 && $currsnum<$snum) ) )
         {
            $score=6;
            $word=$token;
            $snum=$currsnum;
         }
         elsif( ( $wpart eq substr($origin,0,index($origin,"#")) ) && 
         					( $score<4 || ($score=4 && $currsnum<$snum) ) )
         {
            $score=4;
            $word=$token;
            $snum=$currsnum;
         }
         elsif( $score<2 )
         {
            $score=2;
            $word=$token;
            $snum=$currsnum;
         }
      }
      return $word;
}

sub Disambigute {
   my $input = shift;

   my @repv;
   my @repn;
   my @reptoken;

   my $word=undef;
   my $form=undef;
   my $pos=undef;
   my $sense_str=undef;
   my $wordobject=undef;

   my $instance = {
              "contextwords" => [],
              "targetwordobject" => undef
            };

   my @words = split(/[|]/,$input);
   
   my $target=$words[0];   
   ($word, $form, $sense_str, $pos) = split(/[-]/,$target);
   $wordobject=WordNet::SenseRelate::Word->new($word);
   $wordobject->{word}=$word;
   $wordobject->{poslist}=$pos;
   $wordobject->{forms}=[$form];
   if($pos ne "v")
   {
      $wordobject->{senses}=[split(/[,]/,$sense_str)];
      $reptoken[0]=0;
   }
   else
   {
      my @vs=split(/[,]/,$sense_str);
      my @ns=GetNounFromVerb([@vs],$wntools->{wn});
      push @repv,@vs;
      push @repn,@ns;
      $wordobject->{senses}=[@ns];
      $wordobject->{poslist}="n";
      $reptoken[0]=1;
   }
   $instance->{targetwordobject}=$wordobject;

   for (my $i=1; $i < @words; $i++)
   {
      $target=$words[$i];
      ($word, $form, $sense_str, $pos) = split(/[-]/,$target);
      $wordobject=WordNet::SenseRelate::Word->new($word);
      $wordobject->{word}=$word;
      $wordobject->{poslist}=$pos;
      $wordobject->{forms}=[$form];
      if($pos ne "v")
      {
         $wordobject->{senses}=[split(/[,]/,$sense_str)];
         $reptoken[$i]=0;
      }
      else
      {
         my @vs=split(/[,]/,$sense_str);
         my @ns=GetNounFromVerb([@vs],$wntools->{wn});
         push @repv,@vs;
         push @repn,@ns;
         $wordobject->{senses}=[@ns];
         $wordobject->{poslist}="n";
         $reptoken[$i]=1;
      }
      push(@{$instance->{contextwords}},$wordobject);
   }
   
   die "Input string is not valid.\n" if (!defined($instance) || !ref($instance));
 
   my $res=$module->disambiguate($instance);
   my $ret="";
   
   my @tokens1=split(/[::]/,$res);
   my @tokens;
   foreach my $i (0 .. $#tokens1)
   {
      if($tokens1[$i] ne "")
      {
         push(@tokens,$tokens1[$i]);
      }
   }
   
   foreach my $i (0 .. $#tokens)
   {
      if($reptoken[$i]==1)
      {
      l: foreach my $j (0 .. $#repn)
         {
            if($tokens[$i] eq $repn[$j])
            {
               $ret=$ret.$repv[$j]."::";
               last l;
            }
         }
      }
      else
      {
         $ret=$ret.$tokens[$i]."::"
      }
   }
   $ret=substr($ret, 0, -2);
   return $ret;
}
