using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace GnuConvert.Models.FokonyvSzamok
{ 



    public class FokonyvMegmondo
    {
        public string SzamlaElemzes(string partner, string szoveg, string datum)
            {
            var predictor = new InvoicePredictor();
                var  predicted = predictor.Predict(partner, szoveg, datum);
                
                return predicted;
                
            }

        
        
     }
    public class InvoiceTrainer
    {
        private static readonly string modelPath = "model.zip";

        public static void Train()
        {
            var context = new MLContext();

            var rawdata = context.Data.LoadFromTextFile<InvoiceData>("AdatbazisTanitashozTeszt1.csv", hasHeader: false, separatorChar: ';');
            // IDataView->IEnumerable < InvoiceData >
            var dataEnumerable = context.Data.CreateEnumerable<InvoiceData>(rawdata, reuseRowObject: false).ToList();

            // Üres mezők kitöltése
            foreach (var row in dataEnumerable)
            {
                if (string.IsNullOrWhiteSpace(row.Kozlemeny))
                    row.Kozlemeny = "nincs_kozlemeny";
            
            }

            // IEnumerable -> IDataView vissza
            var data = context.Data.LoadFromEnumerable(dataEnumerable);



            var pipeline = context.Transforms.Conversion.MapValueToKey("Label", nameof(InvoiceData.Fokonyvszam))
            .Append(context.Transforms.Text.FeaturizeText("PartnerFeats", nameof(InvoiceData.Partnerneve)))
            .Append(context.Transforms.Text.FeaturizeText("DescFeats", nameof(InvoiceData.Kozlemeny)))
            .Append(context.Transforms.Text.FeaturizeText("DatumFeats", nameof(InvoiceData.Datum)))
            .Append(context.Transforms.Concatenate("Features","PartnerFeats", "DescFeats","DatumFeats"))
            .Append(context.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
            .Append(context.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

           

            var model = pipeline.Fit(data);

            context.Model.Save(model, data.Schema, modelPath);
        }
    }
    public class InvoiceData
    {
        [LoadColumn(0)] public string Partnerneve { get; set; }
      
        [LoadColumn(1)] public string Kozlemeny { get; set; }
       
        [LoadColumn(2)] public string Datum { get; set; } // yyyy.MM.dd
        [LoadColumn(3)] public string Fokonyvszam { get; set; } // ezt fogja megtippelni
        //[LoadColumn(4)] public string TK { get; set; } // label2 (T vagy K)

     
        //// Származtatott jellemzők
        //public float Ev => float.Parse(Datum.Substring(0, 4));
        //public float Honap => float.Parse(Datum.Substring(4, 2));
        //public float Nap => float.Parse(Datum.Substring(7, 2));



    }
    public class InvoicePrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedAccount { get; set; }
    }
    public class InvoicePredictor
    {
        private static readonly string modelPath = "model.zip";

        private readonly PredictionEngine<InvoiceData, InvoicePrediction> _predictionEngine;

        public InvoicePredictor()
        {
            var context = new MLContext();
            if (!File.Exists(modelPath))
            {
                // ha nincs modell, tanítsuk le
                InvoiceTrainer.Train();
            }

            var loadedModel = context.Model.Load(modelPath, out _);
            _predictionEngine = context.Model.CreatePredictionEngine<InvoiceData, InvoicePrediction>(loadedModel);
        }

        public string Predict(string partner, string description,string datum)
        {
            var input = new InvoiceData
            {
                Partnerneve = partner,
                Kozlemeny = description,
                Datum=datum
               // Ar = amount.Replace(',', '.')
            };
            if (description == "" || description == null)
            {
                input.Kozlemeny = "nincs_kozlemeny";
            }
            if (input.Kozlemeny == null)
            {
                input.Kozlemeny = "nincs_kozlemeny";
            }
            var result = _predictionEngine.Predict(input);
            return result.PredictedAccount;
        }
    }
}
