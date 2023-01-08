using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;
using System.Threading;

namespace WebApiFunction.Application
{

    public class TranslationManager
    {
        /*private LanguageTranslationsObject _currentLanguageSet = null;
        private GeneralDefs.LANGUAGE _activeLanguage = GeneralDefs.LANGUAGE.EN;

        public string GetTranslation(string value)
        {
            value = value.Replace(" ", "_");
            foreach (object key in _currentLanguageSet.Keys)
            {
                string strKey = key.ToString();
                if (strKey.Equals(value))
                    return _currentLanguageSet[key].ToString();
            }
            return "ERROR";
        }

        public LanguageTranslationsObject Translations
        {
            get
            {
                return _currentLanguageSet;
            }
        }
        public string GetTranslation(object translationKey)
        {
            if (translationKey == null)
                return null;

            if (_currentLanguageSet.Contains(translationKey))
            {
                return _currentLanguageSet[translationKey].ToString();
            }
            return null;
        }
        private string LanguageCultureCode
        {
            get
            {
                string code = "en-EN";
                switch (_activeLanguage)
                {
                    case GeneralDefs.LANGUAGE.DE:
                        code = "de-DE";
                        break;
                    case GeneralDefs.LANGUAGE.EN:
                        code = "en-EN";
                        break;
                }
                return code;
            }
        }
        private string LanguageDateFormat
        {
            get
            {
                string code = "yyyy-MM-dd HH:mm:ss";
                switch (_activeLanguage)
                {
                    case GeneralDefs.LANGUAGE.DE:
                        code = "dd.MM.yyyy HH:mm:ss";
                        break;
                    case GeneralDefs.LANGUAGE.EN:
                        code = "yyyy-MM-dd HH:mm:ss";
                        break;
                }
                return code;
            }
        }

        private string LanguageDateSeparator
        {
            get
            {
                string code = "/";
                switch (_activeLanguage)
                {
                    case GeneralDefs.LANGUAGE.DE:
                        code = ".";
                        break;
                    case GeneralDefs.LANGUAGE.EN:
                        code = "/";
                        break;
                }
                return code;
            }
        }
        public void SetActiveLanguage(GeneralDefs.LANGUAGE language)
        {
            _activeLanguage = language;

            List<ResourceDictionary> dictionaryList = app.Resources.MergedDictionaries.ToList();

            if (dictionaryList.Count == 0)
                throw new Exception("no resources found");

            List<string> failResources = new List<string>();
            int dictKeyKeysCount = 0;
            for (int i = 0; i < dictionaryList.Count; i++)
            {
                ResourceDictionary resDict = dictionaryList[i];
                bool isLangRes = (resDict.Source.OriginalString.IndexOf("Language/") == -1 ? false : true);
                if (isLangRes)
                {
                    if (dictKeyKeysCount == 0)
                    {
                        if (resDict.Keys.Count == 0)
                        {
                            throw new Exception("no resources found");
                        }
                        else
                        {
                            dictKeyKeysCount = resDict.Keys.Count;
                        }
                    }
                    if (resDict.Keys.Count != dictKeyKeysCount)
                        failResources.Add(resDict.Source.OriginalString);
                }
            }

            if (failResources.Count != 0)
            {
                throw new Exception("the language resources have different key count");
            }

            string requestedCulture = string.Format("Language/StringResources.{0}.xaml", LanguageCultureCode);
            _currentLanguageSet = dictionaryList.FirstOrDefault(d => d.Source.OriginalString == requestedCulture);

            if (_currentLanguageSet == null)
            {
                requestedCulture = "StringResources.en-EN.xaml";//Default is english
                _currentLanguageSet = dictionaryList.FirstOrDefault(d => d.Source.OriginalString == requestedCulture);
            }

            if (_currentLanguageSet != null)
            {
                app.Resources.MergedDictionaries.Remove(_currentLanguageSet);
                app.Resources.MergedDictionaries.Add(_currentLanguageSet);
            }
            NumberFormatInfo numberInfo = CultureInfo.CreateSpecificCulture(LanguageCultureCode).NumberFormat;
            CultureInfo info = new CultureInfo(LanguageCultureCode);
            info.NumberFormat = numberInfo;
            info.DateTimeFormat.DateSeparator = LanguageDateSeparator;
            info.DateTimeFormat.ShortDatePattern = LanguageDateFormat;

            Thread.CurrentThread.CurrentUICulture = info;
            Thread.CurrentThread.CurrentCulture = info;
        }*/
    }
}
