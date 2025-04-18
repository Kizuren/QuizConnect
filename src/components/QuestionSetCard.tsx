import React from 'react';
import { ChevronRight, Lock } from 'lucide-react';
import { QuestionSet } from '../types/api';

interface QuestionSetCardProps {
  questionSet: QuestionSet;
  onClick: () => void;
}

export const QuestionSetCard: React.FC<QuestionSetCardProps> = ({ questionSet, onClick }) => {
  return (
    <button
      onClick={onClick}
      disabled={questionSet.locked}
      className="w-full bg-gray-800 rounded-lg p-6 text-left transition-colors hover:bg-gray-750 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed"
    >
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-lg font-semibold text-white mb-1">
            {questionSet.questionSetName}
          </h3>
          <p className="text-sm text-gray-400">Question Set #{questionSet.questionSetOrder}</p>
        </div>
        {questionSet.locked ? (
          <Lock className="w-5 h-5 text-gray-500" />
        ) : (
          <ChevronRight className="w-5 h-5 text-blue-500" />
        )}
      </div>
    </button>
  );
};