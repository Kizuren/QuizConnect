import React, { useState } from 'react';
import { Button } from './Button';
import { X } from 'lucide-react';

interface QuestionFormProps {
  onSubmit: (data: {
    questionText: string;
    expectedResultText: string;
    minWordLength: number;
    maxWordLength: number;
  }) => void;
  onCancel: () => void;
  initialData?: {
    questionText: string;
    expectedResultText: string;
    minWordLength: number;
    maxWordLength: number;
  };
}

export const QuestionForm: React.FC<QuestionFormProps> = ({
  onSubmit,
  onCancel,
  initialData,
}) => {
  const [formData, setFormData] = useState({
    questionText: initialData?.questionText || '',
    expectedResultText: initialData?.expectedResultText || '',
    minWordLength: initialData?.minWordLength || 50,
    maxWordLength: initialData?.maxWordLength || 500,
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const validateForm = () => {
    const newErrors: Record<string, string> = {};

    if (!formData.questionText.trim()) {
      newErrors.questionText = 'Question text is required';
    }

    if (!formData.expectedResultText.trim()) {
      newErrors.expectedResultText = 'Expected result is required';
    }

    if (formData.minWordLength < 1) {
      newErrors.minWordLength = 'Minimum word length must be at least 1';
    }

    if (formData.maxWordLength <= formData.minWordLength) {
      newErrors.maxWordLength = 'Maximum word length must be greater than minimum';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (validateForm()) {
      onSubmit(formData);
    }
  };

  return (
    <div className="fixed inset-0 flex items-center justify-center z-50">
      <div className="absolute inset-0 bg-black bg-opacity-50" onClick={onCancel} />
      <div className="bg-gray-800 rounded-lg p-6 w-full max-w-2xl relative z-10">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-xl font-bold text-white">
            {initialData ? 'Edit Question' : 'Add New Question'}
          </h2>
          <Button variant="secondary" size="sm" onClick={onCancel}>
            <X className="w-4 h-4" />
          </Button>
        </div>

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label className="block text-sm font-medium text-gray-300 mb-2">
              Question Text
            </label>
            <textarea
              value={formData.questionText}
              onChange={(e) => setFormData({ ...formData, questionText: e.target.value })}
              className={`w-full bg-gray-700 border ${
                errors.questionText ? 'border-red-500' : 'border-gray-600'
              } rounded-lg p-3 text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 transition-colors`}
              rows={4}
              placeholder="Enter the question text..."
            />
            {errors.questionText && (
              <p className="mt-1 text-sm text-red-500">{errors.questionText}</p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-300 mb-2">
              Expected Result
            </label>
            <textarea
              value={formData.expectedResultText}
              onChange={(e) => setFormData({ ...formData, expectedResultText: e.target.value })}
              className={`w-full bg-gray-700 border ${
                errors.expectedResultText ? 'border-red-500' : 'border-gray-600'
              } rounded-lg p-3 text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 transition-colors`}
              rows={4}
              placeholder="Enter the expected result..."
            />
            {errors.expectedResultText && (
              <p className="mt-1 text-sm text-red-500">{errors.expectedResultText}</p>
            )}
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-300 mb-2">
                Minimum Word Length
              </label>
              <input
                type="number"
                value={formData.minWordLength}
                onChange={(e) => setFormData({ ...formData, minWordLength: parseInt(e.target.value) || 0 })}
                className={`w-full bg-gray-700 border ${
                  errors.minWordLength ? 'border-red-500' : 'border-gray-600'
                } rounded-lg px-4 py-2 text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 transition-colors`}
                min="1"
              />
              {errors.minWordLength && (
                <p className="mt-1 text-sm text-red-500">{errors.minWordLength}</p>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-300 mb-2">
                Maximum Word Length
              </label>
              <input
                type="number"
                value={formData.maxWordLength}
                onChange={(e) => setFormData({ ...formData, maxWordLength: parseInt(e.target.value) || 0 })}
                className={`w-full bg-gray-700 border ${
                  errors.maxWordLength ? 'border-red-500' : 'border-gray-600'
                } rounded-lg px-4 py-2 text-white placeholder-gray-400 focus:outline-none focus:border-blue-500 transition-colors`}
                min={formData.minWordLength + 1}
              />
              {errors.maxWordLength && (
                <p className="mt-1 text-sm text-red-500">{errors.maxWordLength}</p>
              )}
            </div>
          </div>

          <div className="flex justify-end gap-4 mt-6">
            <Button variant="secondary" onClick={onCancel}>
              Cancel
            </Button>
            <Button type="submit">
              {initialData ? 'Save Changes' : 'Add Question'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};